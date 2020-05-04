using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components
{
    public interface IManager<InfoType, GameObjectType> where GameObjectType : MBObjectBase
    {
        List<InfoType> Infos { get; set; }

        void InitializeInfos();

        InfoType GetInfoById(string id, bool addIfNotFound = true);

        InfoType GetInfoByObject(GameObjectType gameObject, bool addIfNotFound = true);

        InfoType AddInfo(GameObjectType gameObject, bool force = false);

        void RemoveInfo(string id);

        GameObjectType GetObjectById(string id);

        GameObjectType GetObjectByInfo(InfoType info);

        void UpdateInfos();
    }
}