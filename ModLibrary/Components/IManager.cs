using System.Collections.Generic;
using TaleWorlds.ObjectSystem;

namespace ModLibrary.Components
{
    public interface IManager<InfoType, GameObjectType> where GameObjectType : MBObjectBase
    {
        HashSet<InfoType> Infos { get; set; }

        void InitializeInfos();

        InfoType GetInfo(GameObjectType gameObject);

        InfoType GetInfo(string id);

        void RemoveInfo(string id);

        GameObjectType GetGameObject(string id);

        GameObjectType GetGameObject(InfoType info);

        void UpdateInfos(bool onlyRemoving = false);
    }
}