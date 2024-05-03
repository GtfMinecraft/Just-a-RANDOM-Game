using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    //"data" is the data loaded from local file
    void LoadData(GameData data);

    //save the data to the fields in "data"
    void SaveData(GameData data);
}
