using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviourPunCallbacks
{
    [Header("Dop Info")]
    public HealthBar healthBar;
    [SerializeField] private GameObject dopInfo;
    [SerializeField] private Text dmgText;
    [SerializeField] private Text hpText;
    [HideInInspector] public float unitHP;
    [HideInInspector] public float spawnPeriod;
    [SerializeField] private float attackDistance;

    [Header("Lvl progression")]
    public float[] lvlHP;
    [SerializeField] private float[] lvlDmg;
    [SerializeField] private float[] lvlSpawnPeriod;
    [SerializeField] public float[] lvlCost;
    private int unitLvl;

    private float unitDmg;
    private bool isStunned;
    private bool isDie;
    private bool isVictory;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [HideInInspector] public GameObject target;
    private GameObject otherMainBuilding;
    [HideInInspector] public GameObject mainBuilding;
    private float additionalAttackDistance;
    private float baseAttackDistance;


    void Awake()
    {
        unitDmg = lvlDmg[0];
        unitHP = lvlHP[0];
        spawnPeriod = lvlSpawnPeriod[0];
    }

    public void initStats(int lvl)
    {
        if (unitLvl == lvl) return;
        unitLvl = lvl;
        unitDmg = lvlDmg[lvl];
        unitHP = lvlHP[lvl];
        healthBar.SetSliderMaxValue(unitHP);
        spawnPeriod = lvlSpawnPeriod[lvl];
        photonView.RPC("InitMessage", RpcTarget.Others, lvl);
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (transform.tag == "Left")
        {
            otherMainBuilding = GameObject.FindGameObjectWithTag("RightBase");
            mainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
        }
        else if (transform.tag == "Right")
        {
            otherMainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
            mainBuilding = GameObject.FindGameObjectWithTag("RightBase");
        }
        else
        {
            if (transform.position.x < 0)
            {
                transform.tag = "Left";
                otherMainBuilding = GameObject.FindGameObjectWithTag("RightBase");
                mainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
            }
            else
            {
                transform.tag = "Right";
                otherMainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
                mainBuilding = GameObject.FindGameObjectWithTag("RightBase");
            }
        }
        transform.LookAt(otherMainBuilding.transform);
        baseAttackDistance = attackDistance;
        additionalAttackDistance = attackDistance + 5;
    }

    void Update()
    {
        if (transform.tag == "Left")
        {
            target = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Right"));
        }
        else
        {
            target = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Left"));
        }
        if (target == otherMainBuilding)
        {
            attackDistance = additionalAttackDistance;
        }
        else
        {
            attackDistance = baseAttackDistance;
        }

        if (otherMainBuilding.GetComponent<Spawner>().baseHP <= 0)
        {
            isVictory = true;
        }
        if (mainBuilding.GetComponent<Spawner>().baseHP <= 0)
        {
            isDie = true;
        }

        AnimationPlay();
    }

    public void AddHP(float hp)
    {
        unitHP = Mathf.Clamp(unitHP + hp, unitHP, lvlHP[unitLvl]);
        healthBar.SetSliderValue(unitHP);
    }
    void AnimationSetBool(string trigger)
    {
        foreach (var param in animator.parameters)
        {
            animator.SetBool(param.name, param.name == trigger);
        }
    }

    void AnimationPlay()
    {
        if (isDie)
        {
            dopInfo.SetActive(false);
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Die");
            return;
        }
        if (isVictory)
        {
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Victory");
            return;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackDistance)
        {
            navMeshAgent.updateRotation = true;
            navMeshAgent.SetDestination(target.transform.position);
            AnimationSetBool("Run");
        }
        else if (distance <= attackDistance)
        {
            navMeshAgent.updateRotation = false;
            transform.LookAt(Vector3.Scale(target.transform.position, Vector3.right));
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Attack");
        }
        if (isStunned)
        {
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Stunned");
        }
    }

    GameObject GetClosestEnemy(GameObject[] enemies) // Доработать
    {
        GameObject target = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Unit>().unitHP > 0)
            {
                Vector3 direction = enemy.transform.position - currentPosition;
                float dist = direction.sqrMagnitude;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    target = enemy;
                }
            }
        }
        if (target == null)
        {
            return otherMainBuilding;
        }
        return target;
    }

    public void TakeDmg(float dmg)
    {
        unitHP -= dmg;
        healthBar.SetSliderValue(unitHP);
        if (unitHP <= 0)
        {
            unitHP = 0;
            healthBar.SetSliderValue(unitHP);
            isDie = true;
            photonView.RPC("Deathrattle", RpcTarget.Others);
        }
    }

    public void Attack()
    {
        if (target != null && target != otherMainBuilding)
        {
            target.GetComponent<Unit>().TakeDmg(unitDmg);
        }
        else if (target == otherMainBuilding)
        {
            target.GetComponent<Spawner>().TakeDmg(unitDmg);
        }
    }

    public void ShowDopInfo()
    {
        dopInfo.transform.GetChild(1).gameObject.SetActive(true);
        dopInfo.transform.GetChild(2).gameObject.SetActive(true);
        hpText.text = unitHP.ToString();
        dmgText.text = unitDmg.ToString();
        dopInfo.transform.localScale = new Vector3(5, 5, 5);
    }
    public void UnshowDopInfo()
    {
        dopInfo.transform.localScale = new Vector3(1, 1, 1);
        dopInfo.transform.GetChild(1).gameObject.SetActive(false);
        dopInfo.transform.GetChild(2).gameObject.SetActive(false);
    }

    [PunRPC]
    private void Deathrattle()
    {
        isDie = true;
    }

    [PunRPC]
    private void InitMessage(int lvl)
    {
        initStats(lvl);
    }

    public string GetStats(int lvl)
    {
        return string.Format("Dmg: {0:f2}, Hp: {1:f2}", lvlDmg[lvl], lvlHP[lvl]);
    }
}
