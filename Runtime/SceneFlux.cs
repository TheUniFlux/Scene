/*
Copyright (c) 2023 Xavier Arpa López Thomas Peter ('Kingdox')

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kingdox.UniFlux;
namespace Kingdox.UniFlux.Scenes
{
    public static partial class Key
    {
        private const string _Scene = nameof(SceneFlux)+".";
        public const string Add = _Scene + nameof(Add);
        public const string Remove = _Scene + nameof(Remove);
        public const string OnRemoved = _Scene + nameof(OnRemoved);
        public const string OnAdded = _Scene + nameof(OnAdded);
    }
    public sealed partial class SceneFlux : MonoFlux
    {
        protected override void OnFlux(in bool condition)
        {
            if (condition)
            {
                SceneManager.sceneLoaded += OnSceneAdded;
                SceneManager.sceneUnloaded += OnSceneRemoved;
            }
            else
            {
                SceneManager.sceneLoaded -= OnSceneAdded;
                SceneManager.sceneUnloaded -= OnSceneRemoved;
            }
        }
        private void OnSceneAdded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                Key.OnAdded.Dispatch(scene);
            }
            catch
            {
                //$"ERROR { nameof(OnSceneAdded)}: {scene}, {mode}".Print("red");
            }
        }
        private void OnSceneRemoved(Scene scene)
        {
            try
            {
                Key.OnRemoved.Dispatch(scene);
            }
            catch
            {
                //$"ERROR {nameof(OnSceneRemoved)}: {scene.name}".Print("red");
            }
        }
#if UNITY_EDITOR
        [SerializeField] private List<string> __list_scenes = new List<string>();
        private void OnGUI()
        {
            if (!UnityEngine.Application.isPlaying) return;
            __list_scenes.Clear();
            var activeScene = SceneManager.GetActiveScene();

#pragma warning disable 0618
            var scenes = SceneManager.GetAllScenes();
#pragma warning restore 0618

            foreach (var item in scenes)
            {
                __list_scenes.Add($"[{item.buildIndex}]: {item.name}");
            }
        }
#endif
        [Flux(Key.Add)] private IEnumerator Request_AddAsyncScene(string name)
        {
            yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        }
        [Flux(Key.Add)] private IEnumerator Request_AddAsyncScene(string[] name)
        {
            for (int i = 0; i < name.Length; i++) yield return SceneManager.LoadSceneAsync(name[i], LoadSceneMode.Additive);
        }
        [Flux(Key.Remove)] private IEnumerator Request_RemovesAsyncScene(string name)
        {
            yield return SceneManager.UnloadSceneAsync(name);
        }
        [Flux(Key.Remove)] private IEnumerator Request_RemovesAsyncScene(string[] name)
        {
            for (int i = 0; i < name.Length; i++) yield return SceneManager.UnloadSceneAsync(name[i]);
        }
    }
}
