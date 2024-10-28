using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using MacacaGames;
using MacacaGames.EffectSystem.Model;

namespace MacacaGames
{
    public class DescriptionBuilder : MonoBehaviour
    {
        /// <summary>
        /// 將描述中的params替換成script中的值
        /// </summary>
        /// <param name="description"></param>
        /// <param name="script"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetDescriptionFormat<T>(string description, T script)
        {
            string result = description;
            List<string> parameters = GetTargetParams(result);
            foreach (var parameter in parameters)
            {
                result = result.Replace( "{[" + parameter + "]}", TryGetOnionValue(parameter, script).ToString());
            }
            return result;
        }
        /// <summary>
        /// 撥洋蔥找最下方的值
        /// </summary>
        /// <returns></returns>
        protected static object TryGetOnionValue(string param,object script)
        {
            string[] properties = param.Split('.');
            
            object currentObject = script;
            foreach (var prop in properties)
            {
                object result = TryGetParameter(prop, currentObject);

                if (currentObject == null)
                {
                    return $"[DescriptionBuilder] ReturnValueInScriptByStr | <color=red>#{param} not found in script#</color>";
                }

                currentObject = result;
            }

            return currentObject;
        }
        /// <summary>
        /// 使用反射嘗試拿資料
        /// </summary>
        /// <param name="param"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        protected static object TryGetParameter(string param,object script)
        {
            bool isSuccess = false;
            
            //先撈屬性和變數
            isSuccess = ReturnValueInScriptByStr(param, script, out var result);
            
            //再撈List
            if (!isSuccess)
            {
                isSuccess = ReturnListValueInScriptByStr(param, script, out result);
            }
            
            //再撈字典
            if (!isSuccess)
            {
                isSuccess = ReturnDictionaryValueInScript(param, script, out result);
            }

            return result;
        }

        

        #region 反射

        /// <summary>
        /// 反射取值
        /// </summary>
        /// <param name="param"></param>
        /// <param name="script"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static bool ReturnValueInScriptByStr(string param,object script, out object result)
        {
            //獲取屬性
            PropertyInfo propInfo = script.GetType().GetProperty(param, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (propInfo != null)
            {
                //獲取屬性值並轉換為字符串
                object value = propInfo.GetValue(script);
                result = value;
                return true;
            }
            //如果屬性不存在，嘗試獲取變數
            FieldInfo fieldInfo = script.GetType().GetField(param, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                object value = fieldInfo.GetValue(script);
                result = value;
                return true;
            }

            result = $"[DescriptionBuilder] ReturnValueInScriptByStr | <color=red>#{param} not found in script#</color>";
            return false;
        }

        /// <summary>
        /// 獲取List的值
        /// </summary>
        /// <param name="param"></param>
        /// <param name="script"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static bool ReturnListValueInScriptByStr(string param, object script, out object result)
        {
            var match = Regex.Match(param, @"^([^\[\]]+)\[([0-9]+)\]$");
            if (match.Success)
            {
                // 獲取集合名和索引
                string listName = match.Groups[1].Value;
                int index = int.Parse(match.Groups[2].Value);

                // 使用反射找到集合
                var field = script.GetType().GetField(listName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var property = script.GetType().GetProperty(listName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                object list = field?.GetValue(script) ?? property?.GetValue(script);

                // 嘗試從集合中獲取值
                if (list is IList && index < ((IList)list).Count)
                {
                    result = ((IList)list)[index];
                    return true;
                }
            }

            result = $"[DescriptionBuilder] ReturnListValueInScriptByStr | <color=red>#{param} not found in script#</color>";
            return false;
        }

        /// <summary>
        /// 獲取Dictionary的值
        /// </summary>
        /// <param name="DicParam"></param>
        /// <param name="script"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected static bool ReturnDictionaryValueInScript(string DicParam, object script, out object result)
        {
            result = $"[DescriptionBuilder] ReturnDictionaryValueInScript | <color=red>#{DicParam} not found in script#</color>";
            
            //解析字串來分離出屬性名和key
            int firstBracket = DicParam.IndexOf('[');
            int lastBracket = DicParam.LastIndexOf(']');
            if (firstBracket == -1 || lastBracket == -1 || lastBracket <= firstBracket + 1)
                return false;
            
            //使用反射找到對應的屬性並獲取其值
            string propertyName = DicParam.Substring(0, firstBracket);
            var targetKey = DicParam.Substring(firstBracket + 1, lastBracket - firstBracket - 1);
            if (targetKey.Contains('\"'))
            {
                targetKey = DicParam.Substring(firstBracket + 1, lastBracket - firstBracket - 1).Trim('\"');
            }

            PropertyInfo propertyInfo = script.GetType().GetProperty(propertyName);
            if (propertyInfo == null || string.IsNullOrEmpty(targetKey))
                return false;

            //使用獲取到的字典和鍵來讀取對應的值
            object propertyValue = propertyInfo.GetValue(script);
            if (propertyValue is IDictionary dictionary)
            {
                foreach (var k in dictionary.Keys)
                {
                    if (k.ToString().Contains(targetKey))
                    {
                        result = dictionary[k];
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region 字串解析

        /// <summary>
        /// 在description中提取{[ ]}中的字串
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static List<string> GetTargetParams(string description)
        {
            List<string> targetParams = new List<string>();
            // 為了避免裝飾用的{ }被一併當成需要替換的字串，所以沿用I2替換字串的{[]}
            // gpt寫ㄉ正規表達式來匹配"{[ ]}"中的內容
            Regex regex = new Regex(@"\{\[(.*?)\]\}");
            MatchCollection matches = regex.Matches(description);

            foreach (Match match in matches)
            {
                // 將匹配到的值加入到列表中
                if (match.Groups.Count > 1)
                {
                    targetParams.Add(match.Groups[1].Value); // Groups[1]是第一個括號匹配的內容
                }
            }
            return targetParams;
        }

        #endregion

        

        #region Testing

        public TestClass testClass = new TestClass();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log(GetDescriptionFormat("{[name]} is Lv.{[level]} and hp:{{[hp]}}, Deal {[atk]} damage to enemy. {[sss[0]]} and {[sss[1]]} in sss", testClass));
            }
        }

        #endregion
        
    }
}

public partial class SROptions
{
    private string _desc;

    [Category("DescriptionBuilder")]
    public string desc
    {
        get { return _desc; }
        set { _desc = value; }
    }
    
    [Category("DescriptionBuilder")]
    public void RunDescriptionBuilder()
    {
        TestClass testClass = new TestClass();
        NameTest nameTest = new NameTest("Hehehe");
        Debug.Log(DescriptionBuilder.GetDescriptionFormat(desc, testClass));
    }
}

#region Testing

public class NameTest
{
    public string myName { get; set; }

    public NameTest(string name)
    {
        myName = name;
    }
}

public class TestClass
{
    public int hp;
    public string atk;
    public int level { get; set; }
    public string name { get; set; }
    public NameTest nameTest { get; set; }
    public List<string> sss { get; set; }
    public Dictionary<string, string> dddd { get; set; }
    public Dictionary<int, float> intDic { get; set; }
    
    public Dictionary<string, NameTest> nameTests { get; set; }
    
    public Dictionary<int, NameTest> nameIntTests { get; set; }
    
    public Dictionary<string, List<int>> dicList { get; set; }

    public TestClass()
    {
        hp = 10;
        atk = "50000";
        level = 9999;
        name = "Dio";
        nameTest = new NameTest("Hello World");
        sss = new List<string> { "bb", "eee" };
        dddd = new Dictionary<string, string>()
        {
            {"a", "ss"},
            {"b", "ww"},
        };
        intDic = new Dictionary<int, float>()
        {
            {0, 1.5f},
            {5, 0.9f},
        };
        
        nameTests = new Dictionary<string, NameTest>()
        {
            {"name01", new NameTest("牛燒肉丼")},
            {"name02", new NameTest("超值碗牛丼")},
        };
        nameIntTests = new Dictionary<int, NameTest>()
        {
            {8, new NameTest("好吃炒飯")},
            {66, new NameTest("咖哩乓乓")},
        };
        dicList = new Dictionary<string, List<int>>()
        {
            {"dicList01", new List<int>(){4, 14}},
            {"dicList02", new List<int>(){7777, 66666}},
        };
    }
}

#endregion
