using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;
public class EnemyCombatDecision
{
    public CombatAction combatAction;
    public Character target;

    public EnemyCombatDecision(CombatAction combatAction, Character target)
    {
        this.combatAction = combatAction;
        this.target = target;
    }
}
public class AIManager : MonoBehaviour
{
    [TextArea]
    [SerializeField] string prompt = string.Empty;
    [TextArea]
    [SerializeField] string footer = string.Empty;

    OpenAIApi api = new OpenAIApi();

    List<ChatMessage> messages = new List<ChatMessage>();
    List<string> combatLog = new List<string>();

    public static AIManager Instance;

    private void Awake()
    {
        //if (Instance == null || Instance != this) Destroy(gameObject);
         Instance = this;
    }
    public void AddCombatLog(string log)
    {
        combatLog.Add(log);
    }

    public async Task<EnemyCombatDecision> DecideCombatAction(Character character)
    {
        //Combat log generation
        string combatLogOutput = "\nThis is the combat log so far:\n";

        foreach (string log in combatLog) 
        { 
            combatLogOutput += log + "\n";
        }

        //Player character info (Enemies)
        string playerInfo = "\nCurrent enemy data:\n";

        foreach(Character cha in GameManager.instance.playerTeam)
        {
            playerInfo += $"Name:{cha.aiCharacterName}, Health:{cha.curHp}/{cha.maxHp}\n";
        }

        //Player character info (Enemies)
        string enemyInfo = "\nCurrent allies data:\n";

        foreach (Character cha in GameManager.instance.enemyTeam)
        {
            playerInfo += $"Name:{cha.aiCharacterName}, Health:{cha.curHp}/{cha.maxHp}\n";
        }
        enemyInfo += $"Your are: {character.aiCharacterName}\n";

        //List out combat actions
        string combatActionList = "\nThere are the possible actions that you can take:\n";

        foreach (CombatAction action in character.combatActions)
        {
            combatActionList += $"[{action.aiName}]{action.aiDescription}\n";
        }

        string completePrompt = string.Empty;

        if (messages.Count == 0)
        {
            completePrompt += prompt + "\n";
        }

        completePrompt += combatLog + playerInfo + enemyInfo + combatActionList + footer;

        var message = new ChatMessage()
        {
            Role = "user",
            Content = completePrompt,
        };

        messages.Add(message);

        var request = new CreateChatCompletionRequest
        { 
            Model = "gpt-3.5-turbo",
            Messages = messages
        };

        var response = await api.CreateChatCompletion(request);

        if (response.Choices != null)
        {
            ChatMessage responseMessage = response.Choices[0].Message;
            messages.Add(responseMessage);

            string[] splitResponse = responseMessage.Content.Split(",");

            CombatAction currentAction = null;

            foreach (CombatAction action in character.combatActions)
                if (action.aiName == splitResponse[0])
                    currentAction = action;

            Character target = null;

            foreach (Character cha in GameManager.instance.playerTeam)
                if(cha.aiCharacterName == splitResponse[1])
                    target = cha;

            foreach (Character cha in GameManager.instance.enemyTeam)
                if (cha.aiCharacterName == splitResponse[1])
                    target = cha;

            Debug.Log(response.Choices[0].Message.Content);

            return new EnemyCombatDecision(currentAction, target);
        }
        return null;
    }
}
