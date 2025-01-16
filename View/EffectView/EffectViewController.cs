using System;
using System.Collections.Generic;
using MacacaGames.EffectSystem;
using MacacaGames.EffectSystem.Model;
using UnityEngine;

public class EffectViewController : MonoBehaviour
{
    [SerializeField]
    EffectViewBase TestView;
    
    public static EffectViewController Instance;
    
    public Dictionary<GameObject, Queue<EffectViewBase>> effectViewPool = new Dictionary<GameObject, Queue<EffectViewBase>>();
    
    public Action<EffectViewBase> onEffectViewCreated;

    void Awake()
    {
        // Test
        Init("");
    }
    
    public void Init(string viewJson)
    {
        EffectSystem.Instance.OnEffectAdded += SpawnEffectView;
        Instance = this;
    }

    void SpawnEffectView(EffectInstanceBase effect)
    {
        // EffectViewBase view = Instantiate(TestView);
        effect.OnEffectActive += view.OnActive;
        effect.OnEffectStart += view.OnStart;
        effect.OnEffectEnd += view.OnEnd;
        effect.OnEffectDeactive += view.OnDeactive;
        effect.OnEffectTick += view.OnTick;
        effect.OnCEffectooldownEnd += view.OnCooldownEnd;
        view.Init(effect, new EffectViewInfo());
        
        onEffectViewCreated?.Invoke(view);
    }
    

    public EffectViewBase RequestEffectView(EffectInstanceBase effect, EffectViewInfo viewInfo)
    {
        if (effectViewPool.TryGetValue(viewInfo.prefab, out var q) == false)
        {
            q = new Queue<EffectViewBase>();
            effectViewPool.Add(viewInfo.prefab, q);
        }
        EffectViewBase effectView = null;

        if (q.Count == 0)
        {
            GameObject instance = GameObject.Instantiate(viewInfo.prefab);

            effectView = instance.GetComponent<EffectViewBase>();

            if (effectView != null)
            {
                NormalizeTransform(effectView.transform);
                effectView.Init(effect, viewInfo);
            }
            else
            {
                throw new Exception("[EffectView] 在EffectView上找不到EffectViewBase。");
            }
        }
        else
        {
            effectView = q.Dequeue();
            NormalizeTransform(effectView.transform);
        }
            
        return effectView;
            
        void NormalizeTransform(Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }
    }

    public void RecoveryEffectView(EffectViewBase effectView)
    {
        var queryKey = effectView.viewInfo.prefab;
    
        effectViewPool[queryKey].Enqueue(effectView);
        // effectView.transform.SetParent(effectViewPoolFolder);
    }
}
