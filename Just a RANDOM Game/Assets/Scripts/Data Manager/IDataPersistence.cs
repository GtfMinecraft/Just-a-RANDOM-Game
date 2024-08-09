using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public interface IDataPersistence
{
    //"data" is the data loaded from local file
    void LoadData(GameData data);

    //save the data to the fields in "data"
    void SaveData(GameData data);
}
