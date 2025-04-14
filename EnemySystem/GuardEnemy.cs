using UnityEngine;
using UnityEngine.UI;
using DreamGSoft.Pool;
using System.Collections.Generic;
using DreamGSoft.ProceduralMazeGeneation;
using DreamGSoft.Controller;
using UnityEngine.AI;
using System.Collections;
using DreamGSoft.Weapon;
using DreamGSoft.Audio;
using System;

/* 
    ------------------- DreamGSoft -------------------

    Hi, This is Goutamraj, Thank you for Checking the code! I hope you find it useful. Enjoy!

    --------------------------------------------------
 */

namespace DreamGSoft.Enemy
{
    public class GuardEnemy : IEnemy
    {
        [SerializeField] Slider healthBar;
        [SerializeField] List<Vector3> petrolPointTransforms = new List<Vector3>();
        int currentPatrolIndex = 0;
        private NavMeshAgent navAgent;

        [Header("Script Ref")]
        WeaponManager_GuardEnemy weaponManager_GuardEnemy;
        [SerializeField] Provoker enemyProvoker;
        //Provoker enemyProvoker2;

        [Header("Values")]
        [SerializeField] bool isEngagged = false;
        [SerializeField] bool isAttacking = false;
        [SerializeField] private float petrollingSpeed = 2;
        [SerializeField] private float pursueSpeed = 9;
        [SerializeField] private float maxAttackDistance = 8;
        [SerializeField] private float minAttackDistance = 3;

        [Header("UI")]
        [SerializeField] Transform healthBarUI;
        public override void InjectManager(EnemyManager manager) //Note: Its not a recursion, as its calling base class InjectManager not this.
        {
            base.InjectManager(manager);
        }

        private void Start()
        {
            healthBarUI.gameObject.SetActive(false);
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.enabled = false; //<==Restrict NavMesh to control the position and auto relocate soewhere else while spawning.
            weaponManager_GuardEnemy = GetComponent<WeaponManager_GuardEnemy>();
        }

        private void OnDisable()
        {
            enemyProvoker.OnEnterProvokeRange -= ProvokeMe;
        }

        public override void InitializeEnemy(EnemyManager enemyManager, Vector3 spawnPosition)
        {
            type = data.enemyType;
            health = data.health;
            damagePow = data.damagePow;
            healthBar.value = health / data.health;

            //Registering the Enemy to EnemyManager class to keep track of it.
            InjectManager(enemyManager);
            RegisterSelf();


            //Assiging enemy petrol Points and Setting the state to Petrol;
            petrolPointTransforms.Add(spawnPosition);//Assigning the current position of the enemy
            List<MazeCell> petrolPoints = MazeGenerator.instance.GetANumberOfRandomCells(1,MazeGenerator.instance.GetACellByPostion(spawnPosition));
            foreach(MazeCell cell in petrolPoints)
            {
                petrolPointTransforms.Add(cell.transform.position);
            }

            //Setting the initial state of enemy.
            commonEnemyStates = CommonEnemyStates.Patrolling;

            Debug.Log("Initialize GuardEnemy=>");

            //Subscribing Provoker function
            enemyProvoker.OnEnterProvokeRange += ProvokeMe;
        }

        public override void TakeDamage(float damage) //<== Call this if need to give damage to enemies.
        {
            //Setting Parsue player
            ProvokeMe();

            
            health -= damage;
            healthBar.value = health / data.health;

            if (health <= 0)
            {
                Debug.Log("TakeDamage=> GuardEnemyDead");
                //Playing Enemy eleminate sound.
                AudioManager.instance.PlayAudio(AudioType.EnemyDead);

                //Unregistering the Enemy from EnemyManager
                UnregisterSelf();
                StopAllCoroutines(); //Stopping all coroutine for this enemy.
                EnemyPool.instance.ReturnEnemyObj(type, gameObject); //Returning to Enemy ObjectPool.
            }

            if (health > 0)
            {
                //Show Enemy Helth bar
                StopCoroutine(ShowHealthBarAndHideAfterSometime());
                StartCoroutine(ShowHealthBarAndHideAfterSometime());
            }
        }

        public override void ProvokeMe()
        {
            StartPursuing();
        }


        #region Behaviour

        private void Update()
        {
            switch (commonEnemyStates)
            {
                case CommonEnemyStates.Idle:
                    //Idle(); //<=To be done
                    break;
                case CommonEnemyStates.Patrolling:
                    navAgent.speed = petrollingSpeed;
                    navAgent.enabled = true;
                    Patrol();
                    break;
                case CommonEnemyStates.Alert:
                    //Alert(); //<=To be done if required
                    break;
                case CommonEnemyStates.Pursuing:
                    navAgent.speed = pursueSpeed;
                    Pursue();
                    break;
                case CommonEnemyStates.Investigating:
                    //Investigate(); //<=To be done if required
                    break;
            }
        }


        #region Patrol
        public void Patrol()
        {
            // If no patrol points are assigned, do nothing
            if (petrolPointTransforms.Count == 0) return;

            // Check if the guard has reached the current patrol point
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                // Move to the next patrol point (looping back to the start if needed)
                CallGoToNextPetrollingPointAfterBeingIdealForSometime();
            }
        }
        public void StopPetrolling()
        {
            //Stoping the enemy to move to the next Petrol point.
            if (nextPetrolingPointIEnumerator != null)
            {
                StopCoroutine(nextPetrolingPointIEnumerator);
            }
        }

        IEnumerator nextPetrolingPointIEnumerator = null;
        public void CallGoToNextPetrollingPointAfterBeingIdealForSometime()
        {
            if(nextPetrolingPointIEnumerator != null)
            {
                StopCoroutine(nextPetrolingPointIEnumerator);
            }
            nextPetrolingPointIEnumerator = GoToNextPetrollingPointAfterBeingIdealForSometime();
            StartCoroutine(nextPetrolingPointIEnumerator);
        }
        public IEnumerator GoToNextPetrollingPointAfterBeingIdealForSometime()
        {
            commonEnemyStates = CommonEnemyStates.Idle;
            yield return new WaitForSeconds(UnityEngine.Random.Range(1,4));
            commonEnemyStates = CommonEnemyStates.Patrolling;
            currentPatrolIndex = (currentPatrolIndex + 1) % petrolPointTransforms.Count;
            navAgent.SetDestination(petrolPointTransforms[currentPatrolIndex]);
        }
        #endregion

        #region Pursue

        //Functionality to StartPursuing
        public void StartPursuing()
        {
            if (isEngagged) return; //Return is already engagged.

            //Functionality for getting engagged.
            isEngagged = true;
            navAgent.SetDestination(GameController.instance.playerController.transform.position);
            commonEnemyStates = CommonEnemyStates.Pursuing;
            StopPetrolling();
        }

        //Functionality to Pursue and Attack.
        public void Pursue()
        {
            if (!isEngagged) return;
            float distance = Vector3.Distance(transform.position, GameController.instance.playerController.transform.position);
            if (distance > maxAttackDistance) // 6 is Max Attack Distance
            {
                // follow the player.
                navAgent.isStopped = false;
                navAgent.SetDestination(GameController.instance.playerController.transform.position);

                //Stop Attack if the player moves.
                Attack(false);
            }
            else
            {
                if(distance < minAttackDistance) // 3 is Max Attack Distance //<= also used to avoid enemy get too close to playeror get inside the player.
                {
                    navAgent.isStopped = true;

                    //Call Attack if the player stops
                    Attack(true);
                }
                else
                {
                    CheckBockingObstaclesWithRayCastAndPerformStopOrMove(); 
                }
                //Trigger Attack here.
                //Create a attack functionality ()=>Rotate towards enemy then attack.
            }

            //Rotate towards the player if attacking.
            if (isAttacking)
            {
                RotatePlayerFaceTowardsPlayer();
            }
        }

        private float raycastInterval = 0.5f; // Time between raycasts
        private float lastRaycastTime = 0f;
        public void CheckBockingObstaclesWithRayCastAndPerformStopOrMove()
        {
            if (Time.time - lastRaycastTime >= raycastInterval)
            {
                lastRaycastTime = Time.time;
                
                RaycastHit hit;
                Vector3 directionToPlayer = (GameController.instance.playerController.transform.position - transform.position).normalized;
                bool hasLineOfSight = !Physics.Raycast(transform.position, directionToPlayer, out hit, maxAttackDistance, LayerMask.GetMask("Obstacle"));
                Debug.Log("CheckBockingObstaclesWithRayCastAndPerformStopOrMove: isStop:"+ hasLineOfSight);
                if (hasLineOfSight)
                { 
                    //Stop
                    navAgent.isStopped = true;

                    //Call Attack if the player stops
                    Attack(true);
                }
                else
                {
                    navAgent.isStopped = false;
                    navAgent.SetDestination(GameController.instance.playerController.transform.position);

                    //Stop Attack if the player moves.
                    Attack(false);
                }
            }
        }

        #endregion

        #region Attack
        //Functionality to Attack
        public void Attack(bool boolVal)
        {
            isAttacking = boolVal;
            //Attack
            if (weaponManager_GuardEnemy.isfiring != boolVal)
            {
                weaponManager_GuardEnemy.isfiring = boolVal;

                //Call Shoot only if boolVal is true, else its not required as incase of false its called to stop the shooting.
                if (boolVal)
                {
                    weaponManager_GuardEnemy.Shoot();
                }
            }
        }

        //Functionality to rotate towards player while attacking.
        private void RotatePlayerFaceTowardsPlayer()
        {
            if (isAttacking)
            {
                Vector3 aimDirection = (GameController.instance.playerController.transform.position - transform.position).normalized;
                Quaternion rotateTo = Quaternion.LookRotation(aimDirection, Vector3.up);

                if (aimDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotateTo, 0.15f);
                }
            }
        }
        #endregion


        #endregion

        #region UIWork
        public IEnumerator ShowHealthBarAndHideAfterSometime()
        {
            healthBar.gameObject.SetActive(true);
            yield return new WaitForSeconds(10);
            healthBar.gameObject.SetActive(false);
        }
        #endregion


        public override void ResetMe()
        {
            // we can add some reset funtionality here.
            petrolPointTransforms.Clear();
            currentPatrolIndex = 0;
            isEngagged = false;
            isAttacking = false;
            weaponManager_GuardEnemy.isfiring = false;
            //Setting the UI
            healthBarUI.gameObject.SetActive(false);
        }

    }
}
