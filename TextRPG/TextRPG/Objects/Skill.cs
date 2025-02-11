using System.Text.Json;

namespace TextRPG.Objects
{
    public class Skill
    {
        public int Mana { get; set; }
        public string Name { get; set; }
        public int Percent { get; set; }
        public int GetLevel { get; set; }
        public string Explain { get; set; }
        public bool GetSkill { get; set; }
        public bool IgnoreDefense { get; set; }
        public bool MultiHit { get; set; }

        public static Dictionary<string, Skill> LoadSkillDictionary(string path)
        {
            // JSON 파일을 모두 읽어옴
            string json = System.IO.File.ReadAllText(path);
        
            // Dictionary<string, Skill>로 역직렬화
            Dictionary<string, Skill> skillDict = JsonSerializer.Deserialize<Dictionary<string, Skill>>(json);

            return skillDict;
        }
    }
}