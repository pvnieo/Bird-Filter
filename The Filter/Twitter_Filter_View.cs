using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using Twitter_Filter.Controller;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Twitter_Filter.View
{
    public class TF_View
    {
        MainWindow mainWindow;
        TF_Controller _Controller;

        public TF_View(MainWindow M)
        {
            _Controller = new TF_Controller();
            mainWindow = M;
            mainWindow.UI_NewUser.PreviewKeyDown += UI_NewUser_PreviewKeyDown;
            mainWindow.UI_NewUser.LostFocus += TextBox_LostFocus;
            mainWindow.UI_NewFilterLabel.PreviewKeyDown += UI_NewFilterLabel_PreviewKeyDown;
            mainWindow.UI_NewFilterLabel.LostFocus += TextBox_LostFocus;
            mainWindow.UI_FilterLabelsList.SelectionChanged += UI_FilterLabelsList_SelectionChanged;
            mainWindow.UI_NewFilterKeyword.PreviewKeyDown += UI_NewFilterKeyword_PreviewKeyDown;
            mainWindow.UI_NewFilterKeyword.LostFocus += TextBox_LostFocus;
            mainWindow.MaxTweetPerUser.ValueChanged += MaxTweetPerUser_ValueChanged;
            mainWindow.SearchableArea.ValueChanged += SearchableArea_ValueChanged;
            mainWindow.GoButton.Click += GoButton_Click;

            /// Chargement des paramètres utilisé par l'utilisateur de Twitter Filter
            _Controller.LoadSettings();
            List<string> users = new List<string>();
            List<string> labels = new List<string>();
            users = _Controller.returnUsers();
            labels = _Controller.returnLabels();
            foreach (string user in users)
            {
                _Controller.ListBox_AddElement(mainWindow.UI_UsersNameList, AddTextBlock(_Controller.PrintUserName(user)),
                                AddDeleteButton(mainWindow.UI_UsersNameList, _Controller.RemoveUser), _Controller.Update_DataBase_User);
            }
            foreach (string label in labels)
            {
                _Controller.ListBox_AddElement(mainWindow.UI_FilterLabelsList, AddTextBlock(_Controller.PrintLabel(label)),
                            AddDeleteButton(mainWindow.UI_FilterLabelsList, _Controller.RemoveLabel), _Controller.Update_DataBase_Label);
            }
            ///fin de chargement des paramètres
            ///
        }

        /// <summary>
        /// Fonctions générales
        /// </summary>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txb = (TextBox)sender;
            txb.Text = string.Empty;
            txb.Background = Brushes.White;
        }

        private TextBlock AddTextBlock(string text)
        {
            TextBlock txtb = new TextBlock();
            txtb.Text = text;
            return txtb;
        }

        private Button AddDeleteButton(ListBox hoster, Action<ListBox, int> Remove_from_DB)
        {
            Button btn = new Button();
            btn.Content = "×";
            btn.FontFamily = new FontFamily("Elephant");
            btn.Foreground = new SolidColorBrush(Colors.Red);
            btn.FontWeight = FontWeights.Bold;
            btn.FontStyle = FontStyles.Normal;
            btn.FontSize = 18;
            btn.Padding = new Thickness(0, -3.7, 0, 0);
            btn.Width = 18;
            btn.Height = 18;
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.Click += (s, e) =>
            {
                Button button = (Button)s;
                DockPanel dkp = (DockPanel)button.Parent;
                int index = hoster.Items.IndexOf(dkp);
                Remove_from_DB(hoster, index);
            };
            return btn;
        }

        private Button AddDeleteButtonForKeyword(ListBox hoster, Action<ListBox, int, int> Remove_from_DB)
        {
            Button btn = new Button();
            btn.Content = "×";
            btn.FontFamily = new FontFamily("Elephant");
            btn.Foreground = new SolidColorBrush(Colors.Red);
            btn.FontWeight = FontWeights.Bold;
            btn.FontStyle = FontStyles.Normal;
            btn.FontSize = 18;
            btn.Padding = new Thickness(0, -3, 0, 0);
            btn.Width = 18;
            btn.Height = 18;
            btn.HorizontalAlignment = HorizontalAlignment.Right;
            btn.Click += (s, e) =>
            {
                Button button = (Button)s;
                DockPanel dkp = (DockPanel)button.Parent;
                int index = hoster.Items.IndexOf(dkp);
                Remove_from_DB(hoster, index, mainWindow.UI_FilterLabelsList.SelectedIndex);
            };
            return btn;
        }

        private void Text_Box_Empty_And_Focus(TextBox textBox)
        {
            textBox.Text = string.Empty;
            textBox.Focus();
        }


        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'un utilisateur
        /// </summary>

        private void UI_NewUser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(mainWindow.UI_NewUser.Text))
            {
                if (e.Key == Key.Enter)
                {
                    string userName = mainWindow.UI_NewUser.Text.ToLower();
                    if (_Controller.IsUserNameValid(userName))
                    {
                        if (!_Controller.IsUserNameAdded(userName))
                        {
                            _Controller.AddUserName(userName);
                            _Controller.ListBox_AddElement(mainWindow.UI_UsersNameList, AddTextBlock(_Controller.PrintUserName(userName)),
                                AddDeleteButton(mainWindow.UI_UsersNameList, _Controller.RemoveUser), _Controller.Update_DataBase_User);
                            Text_Box_Empty_And_Focus(mainWindow.UI_NewUser);
                            mainWindow.UI_NewUser.Background = Brushes.White;
                        }
                        else
                        {
                            Text_Box_Empty_And_Focus(mainWindow.UI_NewUser);
                            mainWindow.UI_NewUser.Background = Brushes.White;
                        }
                    }
                    else
                    {
                        mainWindow.UI_NewUser.Background = Brushes.Red;
                        mainWindow.UI_NewUser.Focus();
                    }
                }
            }
            else
            {
                Text_Box_Empty_And_Focus(mainWindow.UI_NewUser);
            }

        }

        

        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'une étiquette
        /// </summary>

        private void UI_NewFilterLabel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(mainWindow.UI_NewFilterLabel.Text))
            {
                if (e.Key == Key.Enter)
                {
                    string label = mainWindow.UI_NewFilterLabel.Text.ToLower();
                    if (!_Controller.IsLabelAdded(label))
                    {
                        _Controller.AddLabel(label);
                        _Controller.ListBox_AddElement(mainWindow.UI_FilterLabelsList, AddTextBlock(_Controller.PrintLabel(label)),
                            AddDeleteButton(mainWindow.UI_FilterLabelsList, _Controller.RemoveLabel), _Controller.Update_DataBase_Label);
                        Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterLabel);
                    }
                    else
                    {
                        Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterLabel);
                    }
                }
            }
            else
            {
                Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterLabel);
            }
        }

        private void UI_FilterLabelsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainWindow.UI_FilterLabelsList.Items.CurrentItem == null)
            {
                mainWindow.UI_NewFilterKeyword.IsEnabled = false;
                _Controller.KeywordsList_Empty(mainWindow.UI_FilterKeywordsList);

            }
            else
            {
                mainWindow.UI_NewFilterKeyword.IsEnabled = true;
                mainWindow.UI_NewFilterKeyword.Focus();
                int labelIndex = mainWindow.UI_FilterLabelsList.SelectedIndex;
                _Controller.KeywordsList_Empty(mainWindow.UI_FilterKeywordsList);
                foreach (string keyword in _Controller.KeywordList(labelIndex))
                {
                    _Controller.ListBox_AddElement(mainWindow.UI_FilterKeywordsList, AddTextBlock(_Controller.PrintLabel(keyword)),
                            AddDeleteButtonForKeyword(mainWindow.UI_FilterKeywordsList, _Controller.RemoveKeyword),
                            _Controller.Update_DataBase_Keyword);
                }
            }
        }

        /// <summary>
        /// Definition des fonctions relatives à l'ajout d'un mot clé
        /// </summary>

        private void UI_NewFilterKeyword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(mainWindow.UI_NewFilterKeyword.Text))
            {
                if (e.Key == Key.Enter)
                {
                    string keyword = mainWindow.UI_NewFilterKeyword.Text.ToLower();
                    int labelIndex = mainWindow.UI_FilterLabelsList.SelectedIndex;

                    if (!_Controller.IsKeywordAddedToLabel(keyword, labelIndex))
                    {
                        _Controller.AddKeywordToLabel(keyword, labelIndex);
                        _Controller.ListBox_AddElement(mainWindow.UI_FilterKeywordsList, AddTextBlock(_Controller.PrintLabel(keyword)),
                            AddDeleteButtonForKeyword(mainWindow.UI_FilterKeywordsList, _Controller.RemoveKeyword), _Controller.Update_DataBase_Keyword);
                        Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterKeyword);
                    }
                    else
                    {
                        Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterKeyword);
                    }
                }
            }
            else
            {
                Text_Box_Empty_And_Focus(mainWindow.UI_NewFilterKeyword);
            }
        }


        /// <summary>
        /// Fonction relative à la définition du maximum de tweets affichés par user
        /// </summary>
        private void MaxTweetPerUser_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _Controller.ChangeValueOfMaxTweetPerUser( (int)mainWindow.MaxTweetPerUser.Value);
        }

        /// <summary>
        /// Fonction relative à la définition du maximum de tweets dans lesquels on va chercher
        /// </summary>

        private void SearchableArea_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _Controller.ChangeValueOfSearchableArea((int)mainWindow.SearchableArea.Value);
        }

        /// <summary>
        /// Fonctions relative au lancement de recherche et affichage des résultats
        /// </summary>



        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(mainWindow.Results.Document.ContentStart, mainWindow.Results.Document.ContentEnd);
            textRange.Text = string.Empty;
             _Controller.Go(mainWindow.Results);
        }




    }
}