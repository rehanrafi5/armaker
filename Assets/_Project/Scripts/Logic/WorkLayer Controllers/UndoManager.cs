using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum UndoAction
{
    Added,
    Removed,
    Modified
}


[System.Serializable]
public class WorkLayerTransformState
{
    public Vector3 LocalPosition;
    public Quaternion LocalRotation;
    public Vector3 LocalScale;

    public WorkLayerTransformState(Transform t)
    {
        LocalPosition = t.localPosition;
        LocalRotation = t.localRotation;
        LocalScale = t.localScale;
    }

    public void ApplyTo(Transform t)
    {
        t.localPosition = LocalPosition;
        t.localRotation = LocalRotation;
        t.localScale = LocalScale;
    }
}


[System.Serializable]
public class WorkLayerSaveState
{
    public UndoAction Action;
    public int InstanceId;

    public Sprite Sprite;
    public RuntimeAnimatorController Animator;
    public VideoClip Video;
    public AudioClip Audio;

    public int SiblingIndex;

    public WorkLayerTransformState Before;
    public WorkLayerTransformState After;

    public WorkLayerSaveState(UndoAction action)
    {
        Action = action;
    }
}


namespace ARMarker
{
    public class UndoManager : BaseSingleton<UndoManager>
    {
        [SerializeField] private Button undoButton;
        [SerializeField] private Button redoButton;

        private readonly Stack<WorkLayerSaveState> past = new();
        private readonly Stack<WorkLayerSaveState> future = new();

        private WorkLayerSaveState current;
        private WorkLayerTransformState cachedBefore;

        private void Start()
        {
            undoButton.onClick.AddListener(Undo);
            redoButton.onClick.AddListener(Redo);
        }

        private void Update()
        {
            undoButton.interactable = current != null || past.Count > 0;
            redoButton.interactable = future.Count > 0;
        }

        // -------------------------------
        // CAPTURE POINTS
        // -------------------------------

        public void CaptureBeforeModify(WorkLayer layer)
        {
            if (layer == null || layer.Data.isTemporary) return;
            cachedBefore = new WorkLayerTransformState(layer.transform);
        }

        public void CaptureAfterModify(WorkLayer layer)
        {
            if (layer == null || layer.Data.isTemporary || cachedBefore == null) return;

            var state = new WorkLayerSaveState(UndoAction.Modified)
            {
                InstanceId = layer.GetInstanceID(),
                Before = cachedBefore,
                After = new WorkLayerTransformState(layer.transform)
            };

            Push(state);
            cachedBefore = null;
        }

        public void RegisterAdd(WorkLayer layer)
        {
            if (layer == null || layer.Data.isTemporary) return;

            var state = new WorkLayerSaveState(UndoAction.Added)
            {
                InstanceId = layer.GetInstanceID(),
                Sprite = layer.Data.sprite,
                Animator = layer.Data.animController,
                Video = layer.Data.videoClip,
                Audio = layer.Data.audioClip,
                After = new WorkLayerTransformState(layer.transform),
                SiblingIndex = layer.transform.GetSiblingIndex()
            };

            Push(state);
        }

        public void RegisterRemove(WorkLayer layer)
        {
            if (layer == null || layer.Data.isTemporary) return;

            var state = new WorkLayerSaveState(UndoAction.Removed)
            {
                InstanceId = layer.GetInstanceID(),
                Sprite = layer.Data.sprite,
                Animator = layer.Data.animController,
                Video = layer.Data.videoClip,
                Audio = layer.Data.audioClip,
                Before = new WorkLayerTransformState(layer.transform),
                SiblingIndex = layer.transform.GetSiblingIndex()
            };

            Push(state);
        }

        // -------------------------------
        // CORE STACK OPS
        // -------------------------------

        private void Push(WorkLayerSaveState state)
        {
            if (current != null)
                past.Push(current);

            current = state;
            future.Clear();
        }

        // -------------------------------
        // UNDO / REDO
        // -------------------------------

        private void Undo()
        {
            if (current == null && past.Count == 0) return;

            var state = current ?? past.Pop();

            ApplyUndo(state);

            future.Push(state);
            current = past.Count > 0 ? past.Pop() : null;
        }

        private void Redo()
        {
            if (future.Count == 0) return;

            var state = future.Pop();
            ApplyRedo(state);

            if (current != null)
                past.Push(current);

            current = state;
        }

        // -------------------------------
        // APPLY LOGIC
        // -------------------------------

        private void ApplyUndo(WorkLayerSaveState state)
        {
            switch (state.Action)
            {
                case UndoAction.Added:
                    RemoveLayerById(state.InstanceId);
                    break;

                case UndoAction.Removed:
                    RecreateLayer(state, true);
                    break;

                case UndoAction.Modified:
                {
                    var layer = GetLayerById(state.InstanceId);
                    if (layer != null && state.Before != null)
                        state.Before.ApplyTo(layer.transform);
                    break;
                }

            }
        }

        private void ApplyRedo(WorkLayerSaveState state)
        {
            switch (state.Action)
            {
                case UndoAction.Added:
                    RecreateLayer(state, false);
                    break;

                case UndoAction.Removed:
                    RemoveLayerById(state.InstanceId);
                    break;

                case UndoAction.Modified:
                {
                    var layer = GetLayerById(state.InstanceId);
                    if (layer != null && state.After != null)
                        state.After.ApplyTo(layer.transform);
                    break;
                }

            }
        }

        // -------------------------------
        // HELPERS
        // -------------------------------

        private void RecreateLayer(WorkLayerSaveState state, bool applyBefore)
        {
            var layer = WorkSpaceSingleton.Instance.AddLayer(state.Sprite, false);

            if (state.Animator) layer.SetUpAnimator(state.Animator);
            if (state.Video) layer.SetUpVideoController(state.Video);
            if (state.Audio) layer.SetUpSFX(state.Audio);

            var tState = applyBefore ? state.Before : state.After;
            tState?.ApplyTo(layer.transform);

            layer.transform.SetSiblingIndex(state.SiblingIndex);
            state.InstanceId = layer.GetInstanceID();
        }

        private void RemoveLayerById(int id)
        {
            var layer = GetLayerById(id);
            if (layer != null)
                WorkSpaceSingleton.Instance.DeleteLayer(layer);
        }

        private WorkLayer GetLayerById(int id)
        {
            foreach (var l in WorkSpaceSingleton.Instance.GetLayers())
                if (l && l.GetInstanceID() == id)
                    return l;

            return null;
        }
    }
}
