using System;
using CoreTweet;
using System.Collections.Generic;
using System.Collections;

namespace Twitter_Filter.Model
{
    /// <summary>
    /// Modélise les tweets et tous leurs caractéristiques(ne sont pas tous utilisées dans le cadre de notre filtre)
    /// , avec un constructeur pour initialiser un tweet
    /// </summary>
    public class Tweet
    {
        public string userName;
        public string screenName;
        public string tweetText;
        DateTime dateTweet;
        bool isTweetContainsPhoto;
        bool isTweetContainsVedio;
        bool isTweetContainsGif;
        string tweetLocalisation;
        int numberOfCommentary;
        int numberOfRetweet;
        public Tweet(Status status)
        {
            userName = status.User.Name;
            screenName = status.User.ScreenName;
            tweetText = status.Text;
        }
    }
    
    public class T_User 
    {
        public string userName { get; set; }
        public string screenName { get; set; }
        
        public T_User(string userN, string screenN)
        {
            userName = userN;
            screenName = screenN;
        }
    }

    public class TF_Label
    {
        public string name { get; set; }
        public List<string> keywords { get; set; }
    }

    public class FilterByKeyword_Properties
    {
        public List<TF_Label> labels { get; set; }
        public List<string> addedUsers { get; set; }
        public int maxTweetPerUser { get; set; }
        public int searchableArea { get; set; }

        public FilterByKeyword_Properties()
        {
            labels = new List<TF_Label>();
            addedUsers = new List<string>();
            maxTweetPerUser = 20;
            searchableArea = 1000;
        }
    }

    public class ListBoxes_Ressource_Items
    {
        public ArrayList userNameList { get; set; }
        public ArrayList labelsList { get; set; }
        public ArrayList keywordsList { get; set; }

        public ListBoxes_Ressource_Items()
        {
            userNameList = new ArrayList();
            labelsList = new ArrayList();
            keywordsList = new ArrayList();
        }
    }
}