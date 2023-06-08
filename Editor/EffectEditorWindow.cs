using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System;
using MacacaGames.EffectSystem.Model;
namespace MacacaGames.EffectSystem.Editor
{
    public class EffectEditorWindow : EditorWindow
    {
        EffectSystem effectSystem => EffectSystem.Instance;
        const string path = "Assets/0_Game/Scripts/Effect";
        List<IEffectableObject> enemyCache = new List<IEffectableObject>();

        [MenuItem("MacacaGames/EffectSystem/Effect Editor Window")]
        static void ShowWindow()
        {
            CreateInstance<EffectEditorWindow>().Show();
        }

        IEffectableObject _currentSelectIEffectableObjectowner;
        IEffectableObject currentSelectIEffectableObjectowner
        {
            get
            {
                return _currentSelectIEffectableObjectowner;
            }
            set
            {
                cloneTree.Q<Label>("ownerName").text = value.ToString();
                _currentSelectIEffectableObjectowner = value;
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

            if (effectSystem != null)
                effectSystem.OnEffectChange = null;
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
                    currentSelectIEffectableObjectowner = t;
                }
                else
                {
                    currentSelectIEffectableObjectowner = null;
                }

                FreshEffectList();
            }
        }

        [SerializeField]
        EffectInfo effectInfo = new EffectInfo();

        VisualElement effectsLocation;
        VisualElement effectsRoot;
        VisualElement effectableObjectsRoot;
        TemplateContainer cloneTree;
        ScrollView currentIffectableScrollView;
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
                currentSelectIEffectableObjectowner = t;
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

            cloneTree.Q("Spliter").AddManipulator(new VisualElementResizer(
                cloneTree.Q("EffectFunc"),
                cloneTree.Q("Content"),
                cloneTree.Q("Spliter"),
                VisualElementResizer.Direction.Horizontal));

            currentIffectableScrollView = cloneTree.Q<ScrollView>("CurrentItems");


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
            effectSystem.AddRequestedEffect(currentSelectIEffectableObjectowner, effectInfo, tags);
            tagField.value = "";
        }
        void AddEffects(IEnumerable<EffectInfo> effectInfos, params string[] tags)
        {
            effectSystem.AddRequestedEffects(currentSelectIEffectableObjectowner, effectInfos, tags);
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
                new Button(() => { effectSystem.RemoveEffect(effect.owner, effect); })
                .AddClass("effect-remove-btn")
                .Add(() =>
                {
                    return new List<VisualElement> { new VisualElement().AddClass("icon").Action(_ => _.style.backgroundImage = EditorGUIUtility.FindTexture("d_TreeEditor.Trash")) };
                })
                );

            return root;
        }

        VisualElement GenerateCurrentIffectableObjects(IEffectableObject effectableObject)
        {
            VisualElement root = new VisualElement();
            var button = new Button().Action(_ => { currentSelectIEffectableObjectowner = effectableObject; });
            button.text = effectableObject.GetDisplayName();
            root.Add(button);
            return root;
        }

        Dictionary<EffectBase, VisualElement> effectElementQuery = new Dictionary<EffectBase, VisualElement>();
        public void FreshEffectList()
        {
            if (effectSystem == null)
            {
                return;
            }
            if (effectSystem != null)
                effectSystem.OnEffectChange = FreshEffectList;

            ClearEffectList();

            effectsRoot = new VisualElement();
            effectElementQuery = new Dictionary<EffectBase, VisualElement>();

            var list = effectSystem.GetEffectQuery(currentSelectIEffectableObjectowner);
            if (list != null)
            {
                foreach (var effectList in effectSystem.GetEffectQuery(currentSelectIEffectableObjectowner).Values)
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
            effectableObjectsRoot = new VisualElement();
            var effectableObjects = effectSystem.GetEffectableObjects();
            if (effectableObjects != null)
            {
                foreach (var item in effectableObjects)
                {
                    VisualElement el = GenerateCurrentIffectableObjects(item);
                    effectableObjectsRoot.Add(el);
                    // effectElementQuery.Add(item, el);

                    effectableObjectsRoot.Add(new VisualElement(), new VisualElement());
                }
                currentIffectableScrollView.Add(effectableObjectsRoot);
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