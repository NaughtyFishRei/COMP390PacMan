﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour {

    public int itemsCapacity;
    public string[] itemsNameList;
    public float laserMaxTime;
    public float laserLastTime;
    public int maxEnergy;
    public int currentEnergy;
    public int energyCost;

    public bool isUsingItem = false;
    public bool isUsingLaser;
    public bool isBoosting = false;

    public GameObject wallBreaker;
    public GameObject portalAInstance;
    public GameObject portalBInstance;
    public GameObject laserChildInstance;
    public GameObject grenadeInstance;
    public GameObject deathParticleSystem;
    public ParticleSystem moveParticleSystem;
    public ParticleSystem boostParticleSystem;
    public Material pacManMaterial;

    public static Slider boostEnergySlider;
    public static Text energyText;
    private Item[] itemsList;
    [SerializeField]private int ownedItems = 0;
    private GameSceneManager gameManagerScript;
    private PlayerMovement playerMovementScript;

	void Start () {
        gameManagerScript=GameObject.Find("GameManager").GetComponent<GameSceneManager>();
        playerMovementScript=this.GetComponent<PlayerMovement>();

        boostEnergySlider=gameManagerScript.boostEnergySlider;
        energyText=gameManagerScript.energyText;

        itemsCapacity=gameManagerScript.itemsCapacity;
        itemsList=new Item[itemsCapacity];

        itemsNameList=new string[itemsCapacity];
        maxEnergy=gameManagerScript.pacmanEnergyCapacity;
        currentEnergy=0;

        boostEnergySlider.maxValue=maxEnergy;
        boostEnergySlider.value=currentEnergy;
        boostParticleSystem.Stop();
    }

    void Update () {
        boostEnergySlider.value=currentEnergy;
        energyText.text="Boost Energy: \n\n"+currentEnergy+"/"+maxEnergy;
        CheckItemButton();
        CheckMovement();

        if (isUsingLaser) {
            gameManagerScript.soundManager.EnableLaserAudio();
            laserLastTime=laserLastTime+Time.deltaTime;
        }
        if (laserLastTime>=laserMaxTime) {
            gameManagerScript.soundManager.DisableLaserAudio();
            isUsingLaser=false;
            isUsingItem=false;
        }
    }

    private void FixedUpdate() {
        if (isUsingLaser) {
            CreateLaserInstance();
        }
        if (isBoosting) {
            currentEnergy--;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag =="PacDot") {
            PacDot pacDotScript = other.GetComponent<PacDot>();
            gameManagerScript.soundManager.PlaySingleAudio();
            gameManagerScript.PacDotIsEaten(pacDotScript.xPos,pacDotScript.zPos);
            gameManagerScript.PlayerGetPacPoint(1);
            if (currentEnergy<maxEnergy) {
                currentEnergy++;
            }
        }
    }

    public GameObject PlayDeathParticleSystem() {
        deathParticleSystem.GetComponent<ParticleSystem>().GetComponent<Renderer>().material=pacManMaterial;
        return Instantiate(deathParticleSystem, transform.position, Quaternion.Euler(-90,0,0));
    }

    public void UseItem(int index) {
        if (index>itemsCapacity-1) {
            Debug.Log("input>capacity");
            return;
        }
        if (itemsList[index]==null) {
            Debug.Log("No available item");
            return;
        }
        isUsingItem=true;
        string itemName = itemsList[index].GetItemName();
        CreateItemInstance(itemName);

        Debug.Log(itemsNameList[index]+" is used");
        itemsList[index]=null;
        itemsNameList[index]=null;
        ownedItems--;
        gameManagerScript.PlayerUseItem(index);

        if(itemName =="Portal") {
            //GetItem("PortalB");
            Item item = ScriptableObject.CreateInstance<Item>();
            item.SetName("PortalB");
            itemsList[index]=item;
            itemsNameList[index]=item.GetItemName();
            ownedItems++;
            gameManagerScript.PlayerGetItem(index, "PortalB");
        }
    }

    public void GetItem(string itemName) {
        Item item = ScriptableObject.CreateInstance<Item>();
        item.SetName(itemName);
        for (int i =0; i<itemsCapacity; i++) {
            if(itemsList[i] ==null) {
                itemsList[i]=item;
                itemsNameList[i]=item.GetItemName();
                ownedItems++;
                gameManagerScript.PlayerGetItem(i, itemName);
                break;
            }
        }
    }

    public void AddEnergy(int number) {
        if(currentEnergy + number >maxEnergy) {
            currentEnergy=maxEnergy;
        } else {
            currentEnergy=currentEnergy+number;
        }
    }

    public void AddEnergyIgnoreLimit(int number) {
        currentEnergy=currentEnergy+number;
    }

    public bool ItemsListHasSpace() {
        return ownedItems<itemsCapacity;
    }

    private void CheckItemButton() {
        if (isUsingItem) {
            Debug.Log("Is Using Item!");
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("1 is pressed");
            UseItem(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("2 is pressed");
            UseItem(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("3 is pressed");
            UseItem(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Debug.Log("4 is pressed");
            UseItem(3);
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            Debug.Log("5 is pressed");
            UseItem(4);
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            Debug.Log("6 is pressed");
            UseItem(5);
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            Debug.Log("7 is pressed");
            UseItem(6);
        } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            Debug.Log("8 is pressed");
            UseItem(7);
        }
    }

    private void CreateItemInstance(string name) {
        if(name =="WallBreaker") {
            CreateWallBreakerInstance();
        } else if(name =="Portal") {
            CreatePortalAInstance();
            isUsingItem=false;
        } else if(name =="PortalB") {
            CreatePortalBInstance();
            isUsingItem=false;
        } else if (name=="Laser") {
            laserLastTime=0;
            isUsingLaser=true;
        } else if(name =="Grenade") {
            CreateGrenadeInstance();
            isUsingItem=false;
        }
    }

    private void CreateWallBreakerInstance() {
        Instantiate(wallBreaker, this.transform);
    }

    private void CreatePortalAInstance() {
        Debug.Log("Portal Instance created!");
        Instantiate(portalAInstance, this.transform.position, Quaternion.Euler(0,0,0));
    }

    private void CreatePortalBInstance() {
        Debug.Log("PortalB Instance created!");
        Instantiate(portalBInstance, this.transform.position, Quaternion.Euler(0, 0, 0));
    }

    private void CreateLaserInstance() {
        Instantiate(laserChildInstance, this.transform.position, this.transform.rotation);
        Debug.Log("Laser Instance created!");
    }

    private void CreateGrenadeInstance() {
        Instantiate(grenadeInstance, this.transform.position + new Vector3(0,0.75f,0), this.transform.rotation);
    }

    public int GetEnergy() {
        return currentEnergy;
    }

    private void CheckMovement() {
        if (currentEnergy>0) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                boostParticleSystem.Play();
                gameManagerScript.soundManager.EnableBoostAudio();
                moveParticleSystem.Stop();
            } else if (Input.GetKeyUp(KeyCode.Space)) {
                boostParticleSystem.Stop();
                gameManagerScript.soundManager.DisableBoostAudio();
                moveParticleSystem.Play();
            }
            if (Input.GetKey(KeyCode.Space)) {
                playerMovementScript.moveSpeed=0.2f;
                isBoosting=true;
            } else {
                playerMovementScript.moveSpeed=0.1f;
                isBoosting=false;
            }
        } else {
            playerMovementScript.moveSpeed=0.1f;
            boostParticleSystem.Stop();
            gameManagerScript.soundManager.DisableBoostAudio();
            if (playerMovementScript.startMovement==true&&moveParticleSystem.isStopped) {
                moveParticleSystem.Play();
            }
            isBoosting=false;
        }
    }

    public Item[] GetItemsList() {
        return itemsList;
    }
}
