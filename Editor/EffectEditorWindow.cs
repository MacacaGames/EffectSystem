using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System;
using MacacaGames.GameSystem;
using MacacaGames.EffectSystem.Model;
namespace MacacaGames.EffectSystem.Editor
{
    public class EffectEditorWindow : EditorWindow
    {
        EffectManager effectManager => ApplicationController.Instance.GetMonobehaviourLifeCycle<EffectManager>();
        const string path = "Assets/0_Game/Scripts/Effect";
        List<IEffectableObject> enemyCache = new List<IEffectableObject>();
        int enemyIndex = 0;


        [MenuItem("MacacaGames/EffectSystem/Effect Editor Window")]
        static void ShowWindow()
        {
            CreateInstance<EffectEditorWindow>().Show();
        }

        IEffectableObject _owner;
        IEffectableObject owner
        {
            get
            {
                // if (_owner == null)
                //     _owner = ApplicationController.Instance.GetGamePlayController().GetGamePlayData<MainGamePlayData>().hero;

                return _owner;
            }
            set
            {
                // cloneTree.Q<Label>("ownerName").text = value.ToString();
                _owner = value;
                FreshEffectList();
            }
        }

        TextField tagField;
        string[] tags
        {
            get { return tagField.value.Split(','); }
            set { tagField.value = string.Join(",", value); }
        }

        TextField searchField;
        string searchText
        {
            set
            {
                searchField.value = value;
                SearchEffect(value);
            }
            get
            {
                return searchField.value;
            }
        }

        private void OnEnable()
        {
            Init();

            FreshEffectList();
        }

        private void OnDisable()
        {

            if (effectManager != null)
                effectManager.OnEffectChange = null;
        }

        private void OnFocus()
        {
            if (Application.isPlaying == false)
            {
                ClearEffectList();
            }
            else
            {
                if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<IEffectableObject>(out var t) == true)
                {
                    owner = t;
                }
                else
                {
                    owner = null;
                }

                FreshEffectList();
            }
        }

        [SerializeField]
        EffectInfo effectInfo = new EffectInfo();

        VisualElement effectsLocation;
        VisualElement effectsRoot;
        TemplateContainer cloneTree;
        void Init()
        {
            //建構
            var root = this.rootVisualElement;

            // Import UXML
            var visualTree = Resources.Load<VisualTreeAsset>("EffectEditorWindow");
            VisualElement visulaElementFromUXML = visualTree.CloneTree();
            var styleSheet = Resources.Load<StyleSheet>("EffectEditorWindow");
            visulaElementFromUXML.styleSheets.Add(styleSheet);
            visulaElementFromUXML.style.flexGrow = 1;

            cloneTree = visualTree.CloneTree();
            cloneTree.style.flexGrow = 1;
            root.Add(cloneTree);

            effectsLocation = cloneTree.Q("Effects");

            tagField = cloneTree.Q<TextField>("TagField");
            cloneTree.Q<Button>("PickCurrent").clickable.clicked += () =>
            {
                var t = Selection.activeGameObject.GetComponent<IEffectableObject>();
                if (t == null)
                {
                    Debug.LogError("Target is not a IEffectableObject ");
                }
                owner = t;
                FreshEffectList();
            };


            cloneTree.Q<Button>("AddEffect").AddClass("effect-add-btn").clickable.clicked += () =>
            {
                if (Application.isPlaying == false)
                {
                    EditorUtility.DisplayDialog("Effect Editor Window", "Only can use in play mode.", "OK");
                }
                else
                {
                    AddEffect(effectInfo);
                }
            };
            cloneTree.Q<Button>("Bake").clickable.clicked += () =>
            {
                var bakeJsonTextField = cloneTree.Q<TextField>("BakeJsonText");
                EffectSystemScriptBacker.BakeAllEffectEnum(bakeJsonTextField.value);
            };

            cloneTree.Q<ObjectField>("EffectGroupField").objectType = typeof(EffectGroup);
            cloneTree.Q<Button>("AddEffect_Group").clickable.clicked += () =>
            {
                EffectGroup group = cloneTree.Q<ObjectField>("EffectGroupField").value as EffectGroup;
                AddEffectGroupData(group);
            };

            searchField = cloneTree.Q<TextField>("SearchText");
            searchField.RegisterValueChangedCallback(_ => searchText = _.newValue);

            cloneTree.Q("icon-search").style.backgroundImage = new StyleBackground(EditorGUIUtility.FindTexture("Search Icon"));

            SerializedProperty effectInfoSO = new SerializedObject(this).FindProperty("effectInfo");
            cloneTree.Q<PropertyField>("EffectInfo").BindProperty(effectInfoSO);

            // cloneTree.Q("Spliter").AddManipulator(new OnionCollections.DataEditor.VisualElementResizer(
            //     cloneTree.Q("EffectFunc"),
            //     cloneTree.Q("Content"),
            //     cloneTree.Q("Spliter"),
            //     OnionCollections.DataEditor.VisualElementResizer.Direction.Horizontal));

        }

        void SearchEffect(string text)
        {
            foreach (var p in effectElementQuery)
            {
                if (p.Key.GetType().Name.ToUpper().Contains(text.ToUpper()))
                {
                    p.Value.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                }
                else if (p.Key.tags.Any(_ => _.ToUpper().Contains(text.ToUpper())))
                {
                    p.Value.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                }
                else
                {
                    p.Value.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                }
            }
        }

        void AddEffect(EffectInfo effectInfo)
        {
            effectManager.AddRequestedEffect(owner, effectInfo, tags);
            tagField.value = "";
        }
        void AddEffects(IEnumerable<EffectInfo> effectInfos, params string[] tags)
        {
            effectManager.AddRequestedEffects(owner, effectInfos, tags);
        }

        public void AddEffectGroupData(EffectGroup effectGroup)
        {
            string groupName = effectGroup.name;
            AddEffects(effectGroup.effects, groupName);
        }

        VisualElement GetEffectElement(EffectBase effect)
        {
            VisualElement root = new VisualElement().AddClass("effect-element");
            root.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            root.Add(new Toggle("").AddClass("effect-round").Action(_ => { _.value = effect.isActive; _.SetEnabled(false); }));

            root.Add(new Label(effect.GetType().Name).AddClass("effect-name"));

            root.Add(new Label($"{effect.input}").AddClass("effect-level").AddClass("effect-round"));

            root.Add(new VisualElement().AddClass("effect-tags").Add(() =>
            {
                return effect.tags.Select(tagName => new Label(tagName).AddClass("effect-round").AddClass("effect-tag"));
            }));

            if (effect.info.activeCondition != EffectSystemScriptable.ActiveCondition.OnEffectStart ||
                effect.info.deactiveCondition != EffectSystemScriptable.DeactiveCondition.None)
            {
                root.Add(new Label($"{effect.info.activeCondition}/{effect.info.deactiveCondition}").AddClass("effect-condition").AddClass("effect-round"));
            }

            if (effect.info.cooldownTime > 0F)
                root.Add(new Label($"CD:{effect.info.cooldownTime}s").AddClass("effect-coldTime").AddClass("effect-round"));


            root.Add(
                new Button(() => { effectManager.RemoveEffect(effect.owner, effect); })
                .AddClass("effect-remove-btn")
                .Add(() =>
                {
                    return new List<VisualElement> { new VisualElement().AddClass("icon").Action(_ => _.style.backgroundImage = EditorGUIUtility.FindTexture("d_TreeEditor.Trash")) };
                })
                );

            return root;
        }

        Dictionary<EffectBase, VisualElement> effectElementQuery = new Dictionary<EffectBase, VisualElement>();
        public void FreshEffectList()
        {
            if (effectManager != null)
                effectManager.OnEffectChange = FreshEffectList;

            ClearEffectList();

            effectsRoot = new VisualElement();
            effectElementQuery = new Dictionary<EffectBase, VisualElement>();

            var list = effectManager.GetEffectQuery(owner);
            if (list != null)
            {
                foreach (var effectList in effectManager.GetEffectQuery(owner).Values)
                {
                    foreach (var effect in effectList)
                    {
                        VisualElement el = GetEffectElement(effect);
                        effectsRoot.Add(el);
                        effectElementQuery.Add(effect, el);

                        effectsRoot.Add(new VisualElement(), new VisualElement());
                    }
                }
                effectsLocation.Add(effectsRoot);
            }
            SearchEffect(searchText);
        }

        void ClearEffectList()
        {

            if (effectsRoot != null && effectsRoot.parent == effectsLocation)
                effectsLocation.Remove(effectsRoot);
        }


    }
}