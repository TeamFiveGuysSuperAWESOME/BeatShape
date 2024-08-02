using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System.Linq;
using Beatboard;
using Unity.VisualScripting;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public BeatboardManager beatboardManager;

        public void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            foreach (var board in boards)
            {
                int points = board["points"];
                Vector2 position = new Vector2(board["position"][0], board["position"][1]);
                float size = board["size"];
                
                beatboardManager.ManageBeatboard(null, -1, points, size, position);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            beatboardManager = FindObjectOfType<BeatboardManager>();

            var jsonFile = File.ReadAllText(Application.dataPath + "/Levels/1/1.json");
            var levelDataJsonNode = JSON.Parse(jsonFile)["Data"];
            
            string levelName = levelDataJsonNode["LevelName"];
            string levelDescription = levelDataJsonNode["LevelDescription"];
            string levelAuthor = levelDataJsonNode["LevelAuthor"];
            int bpm = levelDataJsonNode["Bpm"];
            var boardsJson = levelDataJsonNode["Boards"];
            List<JSONNode> boards = new List<JSONNode>();
            
            for (int i = 1; i <= boardsJson.Count; i++)
            {
                boards.Add(boardsJson["board" + i]);
            }
            CreateBeatboardAtStart(boards);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
    }
}