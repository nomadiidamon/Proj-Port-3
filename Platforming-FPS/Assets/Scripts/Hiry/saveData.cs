using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
[System.Serializable]

public class playerData
{

    public int HP;
    public int speed;
    public int respawnCount;
    public List<gunData> gunList;

    public playerData(playerController player, int respawns)
    {
        HP = player.GetHealth();
        speed = player.GetSpeed();
        this.respawnCount = respawns;

        gunList = new List<gunData>();
        foreach (var gun in player.GetGunList())
        {
            gunList.Add(new gunData(gun));
        }
        

    }

    public void savePlayer()
    {



        playerData data = new playerData(GameObject.FindObjectOfType<playerController>(), respawnCount);
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
            foreach (var gunData in data.gunList)
            {

                var gunStats = player.getGunName(gunData.gunModelName);
                if (gunStats != null)
                {
                    gunList.Add(new gunData(gunStats));
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


}

