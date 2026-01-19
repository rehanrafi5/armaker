using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum UndoAction
{
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
                var activeLayer = WorkSpaceSingleton.Instance.GetActiveLayer();
                bool undoAvailable = (current != null || past.Count > 0)
                                     && (activeLayer == null || !activeLayer.IsLocked);
                bool redoAvailable = (future.Count > 0)
                                     && (activeLayer == null || !activeLayer.IsLocked);

                undoButton.interactable = undoAvailable;
                redoButton.interactable = redoAvailable;
            }

            // -------------------------------
            // CAPTURE POINTS
            // -------------------------------

            public void CaptureBeforeModify(WorkLayer layer)
            {
                if (layer == null || layer.Data.isTemporary || layer.IsLocked)
                    return;

                // Allow capturing even if it’s the first modification after placement
                cachedBefore = new WorkLayerTransformState(layer.transform);
            }

            public void CaptureAfterModify(WorkLayer layer)
            {
                if (layer == null || layer.Data.isTemporary || cachedBefore == null || layer.IsLocked)
                    return;

                var state = new WorkLayerSaveState(UndoAction.Modified)
                {
                    InstanceId = layer.GetInstanceID(),
                    Before = cachedBefore,
                    After = new WorkLayerTransformState(layer.transform)
                };

                Push(state);
                cachedBefore = null;
            }

            
            
            public void ResetUndoRedo()
            {
                past.Clear();
                future.Clear();
                current = null;
                cachedBefore = null;

                // Update buttons
                undoButton.interactable = false;
                redoButton.interactable = false;
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
                if (state.Action != UndoAction.Modified) return;

                var layer = GetLayerById(state.InstanceId);
                if (layer != null && state.Before != null)
                    state.Before.ApplyTo(layer.transform);
            }

            private void ApplyRedo(WorkLayerSaveState state)
            {
                if (state.Action != UndoAction.Modified) return;

                var layer = GetLayerById(state.InstanceId);
                if (layer != null && state.After != null)
                    state.After.ApplyTo(layer.transform);
            }

            // -------------------------------
            // HELPERS
            // -------------------------------

            private WorkLayer GetLayerById(int id)
            {
                foreach (var l in WorkSpaceSingleton.Instance.GetLayers())
                    if (l && l.GetInstanceID() == id)
                        return l;

                return null;
            }
        }
    }
}