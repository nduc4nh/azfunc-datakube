class MessagingList{
    public List<Messaging> messaging {get; set;}
}

class Messaging{
    public Dictionary<string, string> sender {get; set;}
    public Dictionary<string, string> recipient {get; set;}
    public Dictionary<string, object> message {get; set;}

    // public long? timestamp {get; set;}
}
class MessResponse{
    public List<MessagingList> entry {get; set;}
    public string getMessage(){
        // return entry[0].messaging[0].message["text"];
        return "";
    }
    

    // public long getTimestamp(){
    //     return entry[0]["messaging"][0]["timestamp"]
    // }

    public string getSenderId(){
        return entry[0].messaging[0].sender["id"];
    }

    public string getReceiverId(){
        return entry[0].messaging[0].recipient["id"];
    }
}