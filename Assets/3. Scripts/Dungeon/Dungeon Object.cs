
using UnityEngine;


[CreateAssetMenu(menuName = "Dungeon/Dungeon Object")]
public class DungeonObject : ScriptableObject
{
    public int dungeonIndex;
    public string dungeonSceneName;
    public string dungeonName;
    public Sprite bossIcon;
    public string bossNickname;
    public string bossName;

    [TextArea(1, 10)]
    public string hpDesc;
    [TextArea(1, 10)]
    public string damageDesc;
    [TextArea(1, 10)]
    public string speedDesc;
    [TextArea(3, 10)]
    public string additionalDesc;

    [TextArea(5, 10)]
    public string dungeonDescription;

    public DungeonDTO GetDungeonDTO()
    {
        DungeonDTO dungeonDTO = new DungeonDTO(this);
        dungeonDTO.dungeonName = dungeonName;
        dungeonDTO.bossIcon = bossIcon;
        dungeonDTO.bossName = bossName;

        return dungeonDTO;
    }
}

public class DungeonDTO
{
    public string dungeonName;
    public Sprite bossIcon;
    public string bossName;

    public DungeonDTO(DungeonObject dungeonObject)
    {
        // TODO : 나중에 널 체크 해줘야 함. 
    }
}
