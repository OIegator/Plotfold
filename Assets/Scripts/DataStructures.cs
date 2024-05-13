using System.Collections.Generic;

[System.Serializable]
public class CompletionOptions
{
    public bool stream;
    public float temperature;
    public string maxTokens;
}

[System.Serializable]
public class Message
{
    public string role;
    public string text;

    public Message(string role, string text)
    {
        this.role = role;
        this.text = text;
    }
}

[System.Serializable]
public class RequestObject
{
    public string modelUri;
    public CompletionOptions completionOptions;
    public List<Message> messages;

    public RequestObject()
    {
        modelUri = Constants.ModelUri;

        completionOptions = new CompletionOptions
        {
            stream = Constants.IsStream,
            temperature = Constants.Temperature,
            maxTokens = Constants.MaxTokens
        };

        messages = new List<Message>();
    }
}

[System.Serializable]
public class Response
{
    public Result result;
}

[System.Serializable]
public class Result
{
    public List<Alternative> alternatives;
}

[System.Serializable]
public class Alternative
{
    public Message message;
}