/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]

public class playerData
{

    public int HP;
    public int speed;
    public int respawnCount;
    public List<string> gunList;

    public playerData(playerController player, int respawns)
    {

        player = GameObject.FindWithTag("Player").GetComponent<playerController>();

        if (player == null)
        {
            

            Debug.LogError("PlayerController is null when trying to create playerData.");
            return;
        }

        HP = player.GetHealth();
        speed = player.GetSpeed();
        this.respawnCount = respawns;

        gunList = new List<string>();
        foreach (var gun in player.GetGunList())
        {
            gunList.Add(gun.gunName);
        }
        

    }

    public void savePlayer()
    {



        playerData data = new playerData(GameObject.FindWithTag("Player").GetComponent<playerController>(), respawnCount);
        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(Application.persistentDataPath + "/playerData.json", json);
        Debug.Log("Player data saved to " + Application.persistentDataPath + "/playerData.json");

    }

    public void loadPlayer(playerController player)
    {
        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerData data = JsonUtility.FromJson<playerData>(json);

            player.SetHealth(data.HP);
            player.SetSpeed(data.speed);
            gameManager.instance.respawns = data.respawnCount;

            gunList.Clear();
            foreach (var gunName in data.gunList)
            {

                var gunStats = Resources.Load<gunStats>($"Guns/{gunName}");
                if (gunStats != null)
                {
                    player.AddGun(gunStats);
                }

            }
            Debug.Log("Player data loaded from " + path);
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
        }


    }

    


}





[System.Serializable]

public class gunData
{

    public string gunModelName;
    public int shootDamage;
    public float shootRate;
    public int shootDistance;

    public gunData(gunStats gun)
    {

        gunModelName = gun.gunModel.name;
        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDistance = gun.shootDistance;

    }


}*/

