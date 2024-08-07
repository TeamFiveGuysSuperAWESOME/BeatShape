using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;
using System.Linq;
using Beatboard;
using Levels;
using Unity.VisualScripting;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public BeatboardManager beatboardManager;
        private string _levelName;
        private string _levelDescription;
        private string _levelAuthor;
        private int _bpm;
        private List<JSONNode> _boards = new List<JSONNode>();
        private List<JSONNode> _boardsData = new List<JSONNode>();
        

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
            var jsonFile = LevelManager.level1;
            var levelDataJsonNode = JSON.Parse(jsonFile)["Data"];
            var boardsDataJsonNode = JSON.Parse(jsonFile)["Boards"];
            
            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            _bpm = levelDataJsonNode["Bpm"];
            
            var boardsJson = levelDataJsonNode["Boards"];
            _boards = new List<JSONNode>();
            foreach (var board in boardsJson)
            {
                _boards.Add(board);
            }
            CreateBeatboardAtStart(_boards);
            
            foreach (var boardData in boardsDataJsonNode)
            {
                _boardsData.Add(boardData);
                Debug.Log(boardData);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
    }
}