using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject Player;
    private MapGenerator mapGenerator;
    private Map map;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        mapGenerator = GetComponent<MapGenerator>();
        map = mapGenerator.InitializeNewMap();

        Vector3 tile = map.GetFreeTiles().First();

        Instantiate(Player, tile, Quaternion.identity);
    }

    void Update()
    {
        Debug.Log(map.Hello());
    }
}