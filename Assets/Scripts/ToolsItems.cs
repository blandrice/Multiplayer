using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class ToolsItems : NetworkBehaviour
{
    private NetworkVariable<int> playerId = new NetworkVariable<int>();
    
    [SerializeField] private RectTransform hotBarIndex;
    private int corn=0;
    private int maxCorn = 20;
    private int gold=0;
    private int seeds=5;
    private int goldSeeds=2;
    private bool sharpener = false;
    private int lighter=0;
    private int turret=0;
    private bool cornBag=false;
    private int healthPot=0;
    private bool cornCounter = false;
    private int hotbarLocation = 1;


    private bool planting = false;
    [SerializeField] private float resetPlanting;
    private float plantingTimer;
    private NetworkVariable<bool> plantingState= new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private bool donePlanting;

    private bool mining = false;
    [SerializeField] private float resetMiningTime;
    private float miningTimer;
    private NetworkVariable<bool> miningState = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private Image Progress;
    [SerializeField] private Gradient ProgressGrad;
    private NetworkVariable<float> target= new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    private bool scythe = false;
    private bool pickaxe = false;
    private bool shovel = false;
    private bool lighterItem = false;
    private bool turretItem = false;
    private bool seed, goldSeed;
    private int typeSeed = 0;
    
    void Start()
    {
        miningTimer = resetMiningTime;
        Progress.fillAmount = 0;
    }
    

    void Update()
    {
        
        if (plantingState.Value)
        {
            UpdatePlantProgress();
        }
        if (miningState.Value)
        {
            UpdateGoldProgress();
        }
        if (!(plantingState.Value || miningState.Value))
        {
            Progress.fillAmount = 0;
        }
        
        if (!IsOwner)
        {
            
            return;
        }
        
        SwitchTool();
        //PLANTING CODE

        if (seed && seeds > 0 && planting && Input.GetKey(KeyCode.X))
        {
            plantingTimer -= Time.deltaTime;

            ChangePlantStateTrueServerRpc();
        }

        else if (goldSeed && goldSeeds > 0 && planting && Input.GetKey(KeyCode.X))
        {
            plantingTimer -= Time.deltaTime;

            ChangePlantStateTrueServerRpc();

        }
        else
        {
            plantingTimer = resetPlanting;

            ChangePlantStateFalseServerRpc();
        }
        
        if (plantingTimer <= 0)
        {
            donePlanting = true;
            plantingTimer = resetPlanting;
            
        }

        //PICKAXE CODE


        if (pickaxe && mining && Input.GetMouseButton(0))
        {
            miningTimer -= Time.deltaTime;

            ChangeGoldStateTrueServerRpc();
        }
        else
        {
            miningTimer = resetMiningTime;

            ChangeGoldStateFalseServerRpc();
        }
        if (miningTimer <= 0)
        {
            gold++;
            miningTimer = resetMiningTime;
        }
        //----------------------------------------
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void ChangePlantStateTrueServerRpc()
    {
        ChangePlantStateTrueClientRpc();
    }
    [ClientRpc(RequireOwnership = false)]
    private void ChangePlantStateTrueClientRpc()
    {
        plantingState.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlantStateFalseServerRpc()
    {
        ChangePlantStateFalseClientRpc();
    }
    [ClientRpc(RequireOwnership = false)]
    private void ChangePlantStateFalseClientRpc()
    {
        plantingState.Value = false;
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeGoldStateTrueServerRpc()
    {
        ChangeGoldStateTrueClientRpc();
    }
    [ClientRpc(RequireOwnership = false)]
    private void ChangeGoldStateTrueClientRpc()
    {
        miningState.Value = true;
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeGoldStateFalseServerRpc()
    {
        ChangeGoldStateFalseClientRpc();
    }
    [ClientRpc(RequireOwnership = false)]
    private void ChangeGoldStateFalseClientRpc()
    {
        miningState.Value = false;
    }
    private void UpdatePlantProgress()
    {

        UpdatePlantProgressServerRpc();
        
        Progress.fillAmount = target.Value;
        Progress.color = ProgressGrad.Evaluate(target.Value);
    }
    [ServerRpc(RequireOwnership =false)]
    private void UpdatePlantProgressServerRpc()
    {
        UpdatePlantProgressClientRpc();
    }
    [ClientRpc(RequireOwnership = false)]
    private void UpdatePlantProgressClientRpc()
    {
        target.Value = (resetPlanting - plantingTimer) / resetPlanting;
    }
    
    private void UpdateGoldProgress()
    {
        UpdateGoldProgressServerRpc();
        Progress.fillAmount = target.Value;
        Progress.color = ProgressGrad.Evaluate(target.Value);
    }
    [ServerRpc(RequireOwnership =false)]
    private void UpdateGoldProgressServerRpc()
    {
        UpdateGoldProgressClientRpc();
    }
    [ClientRpc(RequireOwnership =false)]
    private void UpdateGoldProgressClientRpc()
    {
        target.Value = (resetMiningTime - miningTimer) / resetMiningTime;
    }
    private void SwitchTool()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hotbarLocation = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hotbarLocation = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hotbarLocation = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            hotbarLocation = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            hotbarLocation = 5;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            typeSeed++;
            typeSeed %= 2;
            plantingTimer = resetPlanting;
        }
        if (typeSeed == 0)
        {
            seed = true;
            goldSeed = false;
        }
        else
        {
            seed = false;
            goldSeed = true;
        }
        float pos = -100 + (hotbarLocation-1 )* 50;

        hotBarIndex.localPosition = new Vector3(pos, hotBarIndex.localPosition.y, hotBarIndex.localPosition.z);
        if (hotbarLocation == 1)
        {
            scythe = true;
            pickaxe = false;
            shovel = false;
            lighterItem = false;
            turretItem = false;
        }
        if (hotbarLocation == 2)
        {
            scythe = false;
            pickaxe = true;
            shovel = false;
            lighterItem = false;
            turretItem = false;
        }
        if (hotbarLocation == 3)
        {
            scythe = false;
            pickaxe = false;
            shovel = true;
            lighterItem = false;
            turretItem = false;
        }
        if (hotbarLocation == 4)
        {
            scythe = false;
            pickaxe = false;
            shovel = false;
            lighterItem = true;
            turretItem = false;
        }
        if (hotbarLocation == 5)
        {
            scythe = false;
            pickaxe = false;
            shovel = false;
            lighterItem = false;
            turretItem = true;
        }

    }
    //OnGUI will be temporary (will replace with canvas because those look nicer)
    private void OnGUI()
    {
        if (!IsOwner) { return; }
        GUI.Box(new Rect(200, 10, 180, 25), "Gold : " + (gold));
        if (seed)
        {
            GUI.Box(new Rect(400, 10, 180, 25), "Seeds : " + (seeds));
        }
        if (goldSeed)
        {
            GUI.Box(new Rect(400, 10, 180, 25), "GoldSeeds : " + (goldSeeds));
        }
    }
    public bool Mining
    {
        get { return mining; }
        set { mining = value; }
    }


    //idk this might be important later :( 
    public bool Scythe
    {
        get { return scythe; }
        
    }
    public bool Pickaxe
    {
        get { return pickaxe; }
        
    }
    public bool Shovel
    {
        get { return shovel; }
        
    }
    public bool Lighter
    {
        get { return lighterItem; }
    }
    public bool Turret
    {
        get { return turretItem; }
    }
    public bool Planting
    {
        get { return planting; }
        set { planting = value; }
    }
    public bool MiningState
    {
        get { return miningState.Value; }
    }
    public bool PlantingState
    {
        get { return plantingState.Value; }
    }
    public bool DonePlanting
    {
        get { return donePlanting; }
        set { donePlanting = value; }
    }
    public bool Seed
    {
        get { return seed; }
    }
    public bool GoldSeed
    {
        get { return goldSeed; }
    }
    public int Seeds
    {
        get { return seeds; }
        set { seeds = value; }
    }
    public int GoldenSeeds
    {
        get { return goldSeeds; }
        set { goldSeeds = value; }
    }
}
