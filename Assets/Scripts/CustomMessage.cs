using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IliveDataRecieved : IEventSystemHandler
{

   void liveDataRecieved(LiveAcmiDataReciever.ACMIdata liveData);

}
