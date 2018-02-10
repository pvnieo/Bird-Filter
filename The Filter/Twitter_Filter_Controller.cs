using CoreTweet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Xml;
using Twitter_Filter.Model;

namespace Twitter_Filter.Controller
{
    public class TF_Controller
    {
        Tokens tokens;
        FilterByKeyword_Properties Filtre_Par_Mot_Cle;
        ListBoxes_Ressource_Items source;
        static SemaphoreSlim doorman = new SemaphoreSlim(1);
        private static Object _lock = new Object();
        static readonly Semaphore Semaphore = new Semaphore(1, 1);
        public TF_Controller()
        {
            // Connecting With the API
            tokens = Tokens.Create("26dm6U2e5F3TK31lHM1ntq18L", "Rkc4rrd5YnrHPKT8xUcOmi1Lrw4Yn" +
                "vovf3kpyluWRZ8qSSehBk", "917866259701731333-7TaOPwVARcVGv57rz2uYVjQs08PwXjK",
                "Za416WqK0AvHzfdiuh8cfXtAryNyETgTdfAyA8eo1jyQE");
            Filtre_Par_Mot_Cle = new FilterByKeyword_Properties();
            source = new ListBoxes_Ressource_Items();
        }



        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'un utilisateur
        /// </summary>
        /// 

        public bool IsUserNameValid(string userName)
        {
            Char delimiter = ' ';
            if (userName.Split(delimiter).Length != 1)
                return false;
            try
            {
                Tweet tweet = new Tweet((tokens.Statuses.UserTimeline(userName, count: 1))[0]);
                return true;
            }
            catch (System.Net.WebException)
            {
                MessageBox.Show("No Internet Connection Found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool IsUserNameAdded(string username)
        {
            if (Filtre_Par_Mot_Cle.addedUsers.Contains(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string PrintUserName(string usr)
        {
            try
            {
                Tweet tweet = new Tweet((tokens.Statuses.UserTimeline(usr, count: 1))[0]);
                string str = string.Empty;
                str = tweet.userName + "  (@" + tweet.screenName + ")";
                return str;
            }
            catch
            {
                MessageBox.Show("No Internet Connection Found","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
                return null;
            }
        }


        public void AddUserName(string userName)
        {
            Filtre_Par_Mot_Cle.addedUsers.Add(userName);
        }

        public void RemoveUser(ListBox lsb, int i)
        {
            Filtre_Par_Mot_Cle.addedUsers.RemoveAt(i);
            source.userNameList.RemoveAt(i);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.userNameList;
        }

        public void ListBox_AddElement(ListBox listBox, TextBlock textBlock, Button btn, Action<ListBox ,DockPanel> UpdateDB)
        {
            DockPanel myDockPanel = new DockPanel();
            myDockPanel.Children.Add(textBlock);
            myDockPanel.Children.Add(btn);
            UpdateDB(listBox, myDockPanel);
        }

        public void Update_DataBase_User(ListBox lsb, DockPanel dkp)
        {
            source.userNameList.Add(dkp);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.userNameList;
        }

        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'un label
        /// </summary>
        /// 

        public bool IsLabelAdded(string label)
        {
            foreach(TF_Label lbl in Filtre_Par_Mot_Cle.labels)
            {
                if (lbl.name == label)
                    return true;
            }
            return false;
        }

        public void AddLabel(string label)
        {
            TF_Label lbl = new TF_Label();
            lbl.name = label;
            lbl.keywords = new List<string>();
            Filtre_Par_Mot_Cle.labels.Add(lbl);
        }

        public string PrintLabel(string label)
        {
            string str = string.Empty;
            str = label.First().ToString().ToUpper() + label.Substring(1).ToLower();
            return str;
        }

        public void RemoveLabel(ListBox lsb, int i)
        {
            Filtre_Par_Mot_Cle.labels.RemoveAt(i);
            source.labelsList.RemoveAt(i);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.labelsList;
        }

        public void Update_DataBase_Label(ListBox lsb, DockPanel dkp)
        {
            source.labelsList.Add(dkp);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.labelsList;
        }

        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'un mot-clé
        /// </summary>
        /// 

        public bool IsKeywordAddedToLabel(string keyword, int index)
        {
            if (Filtre_Par_Mot_Cle.labels[index].keywords.Contains(keyword))
                return true;
            else
                return false;
        }

        public void AddKeywordToLabel(string keyword, int index)
        {
            Filtre_Par_Mot_Cle.labels[index].keywords.Add(keyword);
        }

        public void RemoveKeyword(ListBox lsb, int i, int index)
        {
            Filtre_Par_Mot_Cle.labels[index].keywords.RemoveAt(i);
            source.keywordsList.RemoveAt(i);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.keywordsList;
        }

        public void Update_DataBase_Keyword(ListBox lsb, DockPanel dkp)
        {
            source.keywordsList.Add(dkp);
            lsb.ItemsSource = null;
            lsb.ItemsSource = source.keywordsList;
        }

        public List<string> KeywordList(int index)
        {
            return Filtre_Par_Mot_Cle.labels[index].keywords;
        }

        public void KeywordsList_Empty(ListBox lsb)
        {
            source.keywordsList = new ArrayList();
            lsb.ItemsSource = source.keywordsList;
        }

        /// <summary>
        /// Definition des fonctions relatives au changement du max tweet 
        /// </summary>
        /// 

        public void ChangeValueOfMaxTweetPerUser(int value)
        {
            Filtre_Par_Mot_Cle.maxTweetPerUser = value;
        }

        /// <summary>
        /// Definition des fonctions relatives au changement du searchable area 
        /// </summary>
        ///

        public void ChangeValueOfSearchableArea(int value)
        {
            Filtre_Par_Mot_Cle.searchableArea = value;
        }

        /// <summary>
        /// Fonctions relative au lancement de recherche et affichage des résultats 
        /// </summary>
        ///

        private void ResultsChangeText(RichTextBox Results, string text, SolidColorBrush color, FontWeight fontWeight, FontStyle fontStyle)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                TextRange txt = new TextRange(Results.Document.ContentEnd, Results.Document.ContentEnd);
                txt.Text = text;
                txt.ApplyPropertyValue(TextElement.ForegroundProperty, color);
                txt.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
                txt.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
            }
            );
        }

        public void Go (RichTextBox Results)
        {
            // verifier si la liste des utilisateurs n'est pas vide
            if (Filtre_Par_Mot_Cle.addedUsers.Count == 0)
            {
                ResultsChangeText(Results, "\n No user's added", Brushes.Red, FontWeights.Bold, FontStyles.Normal);
            }
            // verifying that all the labels have keywords
            bool flag = true;
            foreach (TF_Label label in Filtre_Par_Mot_Cle.labels)
            {
                if (label.keywords.Count == 0)
                {
                    flag = false;
                    break;
                }
            }
            if (!flag)
            {
                string str = "\n Please Add keyword(s) to all the labels, or delete the labels you don't want to use anymore";
                ResultsChangeText(Results, str, Brushes.Red, FontWeights.Bold, FontStyles.Normal);
            }
            else
            {
                List<Thread> threads = new List<Thread>();
                foreach (string userName in Filtre_Par_Mot_Cle.addedUsers)
                {
                    Thread th = new Thread(() => Search_Tweets_For_User(userName, Results));
                    th.Start();
                }

               
            }
            SaveSettings();
        }


        public void Search_Tweets_For_User(string userName, RichTextBox Results)
        {
            Semaphore.WaitOne();
            try
            {
                //Semaphore.WaitOne();
                foreach (var status in tokens.Statuses.UserTimeline(userName, count: Filtre_Par_Mot_Cle.searchableArea)
                            .Take(Filtre_Par_Mot_Cle.maxTweetPerUser))
                {
                    Tweet tweet = new Tweet(status);
                    List<string> labels = SatisfiedFilterLabels(tweet.tweetText);
                    if (labels.Count != 0 || Filtre_Par_Mot_Cle.labels.Count == 0)
                    {
                        string x = "[ ";
                        for (int i = 0; i < labels.Count; i++)
                        {
                            x += "\"" + PrintLabel(labels[i]) + "\"" + " ";
                        }
                        x += "]  ";
                        ResultsChangeText(Results, x, Brushes.Red, FontWeights.Regular, FontStyles.Normal);
                        ResultsChangeText(Results, tweet.userName + " : ", Brushes.Blue, FontWeights.Bold, FontStyles.Italic);
                        ResultsChangeText(Results, tweet.tweetText + "\n", Brushes.Black, FontWeights.Regular, FontStyles.Normal);
                    }
                }
                //Semaphore.Release();
            } catch (System.Net.WebException)
            {
                MessageBox.Show("No Internet Connection Found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// retourner les labels satisfaits par un tweet
        /// </summary>
        /// 

        public List<string> SatisfiedFilterLabels(string tweetText)
        {
            List<string> labels = new List<string>();
            foreach (TF_Label label in Filtre_Par_Mot_Cle.labels)
            {
                foreach (string keyword in label.keywords)
                {
                    var keywordSplited = keyword.ToLower().Split(' ');
                    string reg = string.Empty;
                    foreach (string word in keywordSplited)
                    {
                        reg += Regex.Escape(word) + "\\s*";
                    }
                    Regex myReg = new Regex(reg);
                    if (myReg.IsMatch(tweetText.ToLower()) && !labels.Contains(label.name))
                    {
                        labels.Add(label.name);
                        break;
                    }
                }
            }
            return labels;
        }

        /// <summary>
        /// fonctions relatives à la sauvegarde et au chargement des paramètres utilisés par l'utilsateur de l'application
        /// </summary>
        /// 

        public void SaveSettings()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode ROOT = xmlDoc.CreateElement("root");

            XmlNode users = xmlDoc.CreateElement("users");
            XmlNode user;
            foreach (var item in this.Filtre_Par_Mot_Cle.addedUsers)
            {
                user = xmlDoc.CreateElement("user");
                user.InnerText = item;
                users.AppendChild(user);
            }
            ROOT.AppendChild(users);

            XmlNode labels = xmlDoc.CreateElement("labels");
            XmlNode label, keywords, keyword;
            XmlAttribute attr;
            foreach (var item in this.Filtre_Par_Mot_Cle.labels)
            {
                label = xmlDoc.CreateElement("label");
                attr = xmlDoc.CreateAttribute("name");
                attr.Value = item.name;
                label.Attributes.Append(attr);
                keywords = xmlDoc.CreateElement("keywords");
                foreach (var subitem in item.keywords)
                {
                    keyword = xmlDoc.CreateElement("keyword");
                    keyword.InnerText = subitem;
                    keywords.AppendChild(keyword);
                }
                label.AppendChild(keywords);
                labels.AppendChild(label);
            }
            ROOT.AppendChild(labels);

            xmlDoc.AppendChild(ROOT);
            xmlDoc.Save("settings.xml");
        }



        public void LoadSettings()
        {
            TF_Label lbl = null;
            if (!System.IO.File.Exists("settings.xml"))
                return;
            using (XmlReader reader =  XmlReader.Create("settings.xml"))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    reader.Read();
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "user":
                                reader.Read();
                                Filtre_Par_Mot_Cle.addedUsers.Add(reader.Value);
                                break;

                            case "label":
                                lbl = new TF_Label();
                                reader.MoveToFirstAttribute();
                                lbl.name = reader.Value;
                                lbl.keywords = new List<string> { };
                                Filtre_Par_Mot_Cle.labels.Add(lbl);
                                break;
                            case "keyword":
                                if (lbl != null)
                                {
                                    reader.Read();
                                    lbl.keywords.Add(reader.Value);
                                }
                                break;
                        }
                    }
                }
            }
        }

        public List<string> returnUsers()
        {
            return Filtre_Par_Mot_Cle.addedUsers;
        }

        public List<string> returnLabels()
        {
            List<string> labels = new List<string>();
            foreach(TF_Label label in Filtre_Par_Mot_Cle.labels)
            {
                labels.Add(label.name);
            }
            return labels;
        }
    }
}