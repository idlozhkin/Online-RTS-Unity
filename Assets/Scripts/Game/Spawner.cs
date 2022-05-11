using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;

public class Spawner : MonoBehaviourPunCallbacks
{
    public float baseHP;
    public HealthBar healthBar;
    [HideInInspector] public int team;
    [HideInInspector] public GameObject[,] units;
    [HideInInspector] public GameObject[,] otherUnits;
    [SerializeField] private Text goldText;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject gameInfo;
    [HideInInspector] public int[] lvls = { 0, 0, 0, 0 };
    [HideInInspector] public Transform[] spanwPoints;
    [SerializeField] private GameObject otherMainBuilding;
    [SerializeField] private Text endGameText;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Text[] mainUIText;
    private float gold;
    private IEnumerator[] coroutine = new IEnumerator[4];
    private IEnumerator[] otherCoroutine = new IEnumerator[4];
    private IEnumerator[] delayCoroutine = new IEnumerator[4];
    private IEnumerator[] otherDelayCoroutine = new IEnumerator[4];
    //private PhotonView photonView;
    private GameInfo gmInfo;
    private int otherTeam;
    private float bonusGold = 3;

    void Awake()
    {
        GetSpawnTransform();

        healthBar.SetSliderMaxValue(baseHP);

        gmInfo = gameInfo.GetComponent<GameInfo>();
        gold = 100;
        goldText.text = gold.ToString();

        if (PhotonNetwork.IsMasterClient)
        {
            team = Random.Range(0, 3);
            //otherTeam = Random.Range(0, 3);
            photonView.RPC("SendData", RpcTarget.Others, team);
            units = gmInfo.GetUnitsArr(team);
            SetTextUI();
        }
    }
    void Start()
    {
        otherUnits = gmInfo.GetUnitsArr(otherMainBuilding.GetComponent<Spawner>().team);
        StartCoroutine(Income(bonusGold));

        if (PhotonNetwork.IsMasterClient && transform.tag.Equals("RightBase"))
        {
            photonView.RPC("SendOtherData", RpcTarget.Others, otherMainBuilding.GetComponent<Spawner>().team);
        }
    }

    public void SetTextUI()
    {
        for (int i = 0; i < 4; i++)
        {
            Unit u = units[i, lvls[i]].GetComponent<Unit>();
            mainUIText[i].text = $"{u.name} \n {u.GetStats(lvls[i])}";
        }
    }


    public void GetSpawnTransform()
    {
        spanwPoints = new Transform[4];
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            spanwPoints[i] = transform.GetChild(i);
        }
    }
    public void TakeDmg(float dmg)
    {
        baseHP -= dmg;
        healthBar.SetSliderValue(baseHP);
        if (baseHP <= 0)
        {
            baseHP = 0;
            healthBar.SetSliderValue(baseHP);
            photonView.RPC("EndGame", RpcTarget.All, transform.tag);
        }
    }

    private IEnumerator Income(float dopGold)
    {
        while (!gmInfo.endGame)
        {
            yield return new WaitForSeconds(5);
            AddGold(dopGold);
        }
    }

    public void AddGold(float dopGold)
    {
        gold += dopGold;
        goldText.text = gold.ToString();
    }

    public void StartSpawner(int id)
    {
        float prepare = 3;

        Unit u = units[id, lvls[id]].GetComponent<Unit>();
        // if (u.lvlCost[lvls[id]] <= gold)
        // {
        //     photonView.RPC("SendCourutine", RpcTarget.Others, id, timer.time + prepare);
        // }
        if (delayCoroutine[id] != null) StopCoroutine(delayCoroutine[id]);
        delayCoroutine[id] = Delay(id, prepare);
        StartCoroutine(delayCoroutine[id]);
    }

    public void StartOtherSpawner(int id, float prepare)
    {
        if (otherDelayCoroutine[id] != null) StopCoroutine(otherDelayCoroutine[id]);
        otherDelayCoroutine[id] = OtherDelay(id, prepare);
        StartCoroutine(otherDelayCoroutine[id]);
    }

    public IEnumerator Delay(int id, float prepare)
    {
        Unit u = units[id, lvls[id]].GetComponent<Unit>();
        if (u.lvlCost[lvls[id]] <= gold)
        {
            gold -= u.lvlCost[lvls[id]];
            if (lvls[id] < 4)
            {
                lvls[id]++;
                SetTextUI();
            }
            if (lvls[id] == 4)
                prepare = 0;
            goldText.text = gold.ToString();
            yield return new WaitForSeconds(prepare);
            if (coroutine[id] != null) StopCoroutine(coroutine[id]);
            coroutine[id] = Spawn(id);
            StartCoroutine(coroutine[id]);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
    }

    public IEnumerator OtherDelay(int id, float prepare)
    {
        yield return new WaitForSeconds(prepare);
        if (otherCoroutine[id] != null) StopCoroutine(otherCoroutine[id]);
        otherCoroutine[id] = OtherSpawn(id);
        StartCoroutine(otherCoroutine[id]);
    }

    public IEnumerator Spawn(int id)
    {
        while (!gmInfo.endGame)
        {
            GameObject unit = PhotonNetwork.Instantiate(units[id, lvls[id]].name, spanwPoints[id].position, Quaternion.identity);
            unit.GetComponent<Unit>().initStats(lvls[id]);
            if (transform.tag == "LeftBase")
            {
                unit.tag = "Left";
            }
            else
            {
                unit.tag = "Right";
            }
            float spawnPeriod = unit.GetComponent<Unit>().spawnPeriod;
            yield return new WaitForSeconds(spawnPeriod);
        }
    }

    public IEnumerator OtherSpawn(int id)
    {
        Spawner s = otherMainBuilding.GetComponent<Spawner>();
        while (!gmInfo.endGame)
        {
            GameObject unit = Instantiate(otherUnits[id, s.lvls[id]], s.spanwPoints[id].position, Quaternion.identity);

            unit.GetComponent<Unit>().initStats(s.lvls[id]);
            if (transform.tag == "LeftBase")
            {
                unit.tag = "Right";
            }
            else
            {
                unit.tag = "Left";
            }
            float spawnPeriod = unit.GetComponent<Unit>().spawnPeriod;
            yield return new WaitForSeconds(spawnPeriod);
        }
    }

    [PunRPC]
    private void SendData(int team)
    {
        this.team = team;
        units = gmInfo.GetUnitsArr(team);
        SetTextUI();
        //otherUnits = gmInfo.GetUnitsArr(otherTeam);
        //otherUnits = gmInfo.GetUnitsArr(team);
        // otherMainBuilding.GetComponent<Spawner>().team = team;
    }
    [PunRPC]
    private void SendOtherData(int team)
    {
        //units = gmInfo.GetUnitsArr(team);
        otherUnits = gmInfo.GetUnitsArr(team);
        //otherUnits = gmInfo.GetUnitsArr(team);
        // otherMainBuilding.GetComponent<Spawner>().team = team;
    }

    [PunRPC]
    private void SendCourutine(int id, float time)
    {

        time -= timer.time;
        if (time < 0) time = 0;
        otherMainBuilding.GetComponent<Spawner>().StartOtherSpawner(id, time);
    }


    [PunRPC]
    public void EndGame(string tag)
    {
        gmInfo.endGame = true;
        if (tag == "RightBase")
        {
            endGameText.text = PhotonNetwork.PlayerList[0].NickName + " win";
        }
        else
        {
            endGameText.text = PhotonNetwork.PlayerList[1].NickName + " win";
        }
        endGamePanel.SetActive(true);
        healthBar.gameObject.SetActive(false);
        //StopAllCoroutines();
        //Time.timeScale = 0;
    }

}
