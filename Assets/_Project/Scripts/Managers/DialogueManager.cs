using System.Collections.Generic;
using _Project.Scripts.Data.StaticModels;
using _Project.Scripts.Services.Data;
using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private ConversationData _currentConversation;
        private ConversationNodeData _currentNode;
        private Dictionary<string, ConversationNodeData> _nodesById;

        public bool IsConversationActive => _currentConversation != null;

        public ConversationData CurrentConversation => _currentConversation;
        public ConversationNodeData CurrentNode => _currentNode;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Inicia una conversación a partir de su ID.
        /// </summary>
        public void StartConversation(string conversationId)
        {
            var conv = StaticDataService.Instance.GetConversationById(conversationId);
            if (conv == null)
            {
                Debug.LogError($"[DialogueManager] Conversación no encontrada: {conversationId}");
                return;
            }

            _currentConversation = conv;
            _nodesById = new Dictionary<string, ConversationNodeData>();
            foreach (var node in conv.nodes)
            {
                if (!string.IsNullOrEmpty(node.id))
                    _nodesById[node.id] = node;
            }

            // Asumimos que siempre empieza en "n1"
            if (!_nodesById.TryGetValue("n1", out _currentNode))
            {
                Debug.LogError($"[DialogueManager] Conversación {conversationId} no tiene nodo n1.");
                _currentConversation = null;
                _nodesById = null;
                return;
            }

            #if UNITY_EDITOR
                Debug.Log($"[DialogueManager] Iniciando conversación: {conversationId}");
            #endif

            // Aquí es donde tu UI debería leer DialogueManager.CurrentNode y mostrarla
        }

        /// <summary>
        /// Avanza al siguiente nodo. Si el nextId es "end", termina la conversación.
        /// </summary>
        public void GoToNextNode()
        {
            if (_currentConversation == null || _currentNode == null)
            {
                Debug.LogWarning("[DialogueManager] No hay conversación activa.");
                return;
            }

            // Aplicar flags del nodo actual al salir
            ApplyFlagsFromNode(_currentNode);

            var nextId = _currentNode.nextId;

            if (string.IsNullOrEmpty(nextId) || nextId == "end")
            {
                EndConversation();
                return;
            }

            if (!_nodesById.TryGetValue(nextId, out var nextNode))
            {
                Debug.LogError($"[DialogueManager] nextId '{nextId}' no encontrado en conversación '{_currentConversation.id}'.");
                EndConversation();
                return;
            }

            _currentNode = nextNode;

            #if UNITY_EDITOR
                Debug.Log($"[DialogueManager] Avanzando a nodo: {nextId}");
            #endif

            // Tu UI debería actualizarse leyendo el nuevo CurrentNode
        }

        /// <summary>
        /// Termina la conversación actual y limpia el estado.
        /// </summary>
        public void EndConversation()
        {
            #if UNITY_EDITOR
                if (_currentConversation != null)
                    Debug.Log($"[DialogueManager] Terminando conversación: {_currentConversation.id}");
            #endif

            _currentConversation = null;
            _currentNode = null;
            _nodesById = null;

            // Aquí podrías notificar a la UI para ocultar la ventana de diálogo.
        }

        private void ApplyFlagsFromNode(ConversationNodeData node)
        {
            if (node.flagsOnComplete == null || node.flagsOnComplete.Length == 0)
                return;

            foreach (var flagId in node.flagsOnComplete)
            {
                if (!string.IsNullOrEmpty(flagId))
                {
                    FlagsManager.Instance.RaiseFlag(flagId);
                }
            }
        }
    }
}