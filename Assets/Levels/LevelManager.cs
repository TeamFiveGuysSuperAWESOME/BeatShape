using File = System.IO.File;

namespace Levels
{
    public class LevelManager
    {
        public static string level1 =
            "{\n    \"Data\" : {\n        \"LevelName\" : \"Test1\",\n        \"LevelDescription\" : \"Test Level\",\n        \"LevelAuthor\" : \"Test Author\",\n        \"Bpm\" : 100,\n        \"Boards\" : {\n            \"board1\" : {\"points\" : 4, \"position\" : [-100,0], \"size\" : 40},\n            \"board2\" : {\"points\" : 6, \"position\" : [50,0], \"size\" : 20}\n        }\n    },\n    \n    \"Boards\" : {\n        \"board1\" : {\n            \"Circle1\" : {\n                \"Beat1\" : {\n                    \"Beat\" : true\n                },\n                 \"Beat2\" : {\n                    \"Beat\" : true\n                 },\n                 \"Beat3\" : {\n                },\n                \"Beat4\" : {\n                }\n            },\n            \"Circle2\" : {\n                \"Beat1\": {\n                },\n                \"Beat2\": {\n                },\n                \"Beat3\": {\n                },\n                \"Beat4\": {\n                }\n            }    \n        },\n        \"board2\" : {\n            \"Circle1\" : {\n                \"Beat1\" : {\n                    \"Beat\" : true\n                },\n                \"Beat2\" : {\n                    \"Beat\" : true\n                },\n                \"Beat3\" : {\n                },\n                \"Beat4\" : {\n                }\n            },\n            \"Circle2\" : {\n                \"Beat1\": {\n                },\n                \"Beat2\": {\n                },\n                \"Beat3\": {\n                },\n                \"Beat4\": {\n                }\n            }\n        }\n    }\n}";
    }
}