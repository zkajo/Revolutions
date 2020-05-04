using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components
{
    public interface IManager<InfoType, GameObjectType> where GameObjectType : MBObjectBase
    {
        List<InfoType> Infos { get; set; }

        void InitializeInfos();

        InfoType GetInfoById(string id);

        InfoType GetInfoByObject(GameObjectType gameObject);

        InfoType AddInfo(GameObjectType gameObject);

        void RemoveInfo(string id);

        GameObjectType GetObjectById(string id);

        GameObjectType GetObjectByInfo(InfoType info);

        void UpdateInfos();
    }
}