﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _8_Ball_League
{
    public partial class Form2 : Form
    {
        int maxlabsiz = 0;

        List<string> playernames;
        List<float> playerrankings;

        List<string> fullplayernames;
        List<float> fullplayerrankings;

        List<float> playerwins;

        Random rnd = new Random();

        List<roundinfo> rounds = new List<roundinfo>();
        List<playerinfo> players = new List<playerinfo>();

        public Form1 form1;

        bool allowearlyrematches = false;   // Whether to allow rematches before necessary
        List<bool> isplaying = new List<bool>();
        List<int> byes = new List<int>();

        private struct playerinfo
        {
            public List<int> amountplayed;
        }

        private struct roundinfo
        {
            public List<matchinfo> matches;
        }

        private struct matchinfo
        {
            public int p1index;
            public int p2index;
            public float p1score;
            public float p2score;
            public bool finished;

            public List<Control> controls;
            public TextBox p1txt;
            public TextBox p2txt;
        }

        private List<T> newlist<T>(int capacity, T defaultvalue)
        {
            List<T> lst = new List<T>();
            for (int i = 0; i < capacity; i++)
            {
                lst.Add(defaultvalue);
            }

            return lst;
        }

        public void init(List<string> pn, List<float> pr)
        {
            init(pn, pr, pn, pr);
        }

        public void init(List<string> pn, List<float> pr, List<string> fpn, List<float> fpr)
        {

            resize_panel();

            playernames = pn;
            playerrankings = pr;

            for(int i = 0; i < playernames.Count; i++)
            {
                isplaying.Add(true);
                byes.Add(0);   
            }

            fullplayernames = fpn;
            fullplayerrankings = fpr;

            playerwins = new List<float>();

            for (int i = 0; i < playernames.Count; i++)
            {
                playerwins.Add(0);

                playerinfo pi = new playerinfo();

                pi.amountplayed = newlist<int>(playernames.Count, 0);

                players.Add(pi);
            }

            initmatchup();
            updateUI(0);
        }

        private void initmatchup() // Creates a set of random matches for round 1
        {
            roundinfo initround = new roundinfo();
            initround.matches = new List<matchinfo>();
            int next = 0;
            matchinfo match = new matchinfo();



            List<int> plind = new List<int>();

            for (int i = 0; i < playernames.Count; i++)
            {
                plind.Add(i);
            }

            if (plind.Count % 2 != 0) // If the number of playing players are odd
            {
                match = new matchinfo();
                next = rnd.Next(0, plind.Count); // Random player chosen to take the bye
                match.p1index = plind[next];
                match.p2index = -1; //-1 = bye
                plind.RemoveAt(next);
                initround.matches.Add(match);
            }

            int nummatch = plind.Count / 2;

            for (int i = 0; i < nummatch; i++)
            {
                match = new matchinfo();
                next = rnd.Next(0, plind.Count);
                match.p1index = plind[next];
                plind.RemoveAt(next);
                next = rnd.Next(0, plind.Count);
                match.p2index = plind[next];
                plind.RemoveAt(next);
                initround.matches.Add(match);
            }

            rounds.Add(initround);
        }

        private void updateUI(int roundnumber)
        {
            int gap = 60;
            int frontxgap = 20 - masterpanel.HorizontalScroll.Value;
            roundinfo currentround = rounds[roundnumber];
            int labh = 20;
            int labg = 30;

            int xgap = roundnumber * (maxlabsiz + 20 + 60);

            int ygap = 10 - masterpanel.VerticalScroll.Value;

            Label roundnum = new Label();
            roundnum.Font = new Font("Arial", 14, FontStyle.Bold);
            roundnum.Text = "Round " + (roundnumber + 1);
            roundnum.Location = new Point(frontxgap + xgap, ygap);
            roundnum.AutoSize = true;

            masterpanel.Controls.Add(roundnum);

            for (int i = 0; i < currentround.matches.Count; i++)
            {
                matchinfo currentmatch = currentround.matches[i];

                currentmatch.controls = new List<Control>();

                int panelypos = masterpanel.VerticalScroll.Value;

                int y1pos = i * (gap + (2 * labh) + labg) + gap + (labh / 2) - panelypos;
                int y2pos = y1pos + labh + labg;

                int ympos = y1pos - 30;

                Label matchnum = new Label();
                matchnum.Font = new Font("Arial", 12, FontStyle.Underline);
                matchnum.Text = "Match " + (i + 1);
                matchnum.Location = new Point(frontxgap + xgap, ympos);
                matchnum.AutoSize = true;

                masterpanel.Controls.Add(matchnum);

                Label lab1 = new Label();
                lab1.Font = new Font("Arial", 12, FontStyle.Regular);
                lab1.Text = playernames[currentmatch.p1index];
                lab1.Location = new Point(frontxgap + xgap, y1pos);
                lab1.AutoSize = true;
                masterpanel.Controls.Add(lab1);
                currentmatch.controls.Add(lab1);

                Label lab2 = new Label();
                lab2.Font = new Font("Arial", 12, FontStyle.Regular);
                lab2.AutoSize = true;

                if (currentmatch.p2index == -1)
                {
                    lab2.Text = "Bye";
                }

                else
                {
                    lab2.Text = playernames[currentmatch.p2index];
                }

                lab2.Location = new Point(frontxgap + xgap, y2pos);
                masterpanel.Controls.Add(lab2);
                currentmatch.controls.Add(lab2);

                currentround.matches[i] = currentmatch;

                if (maxlabsiz < lab1.Size.Width)
                {
                    maxlabsiz = lab1.Size.Width;
                }

                if (maxlabsiz < lab2.Size.Width)
                {
                    maxlabsiz = lab2.Size.Width;
                }

            }

            for (int i = 0; i < currentround.matches.Count; i++)
            {
                matchinfo currentmatch = currentround.matches[i];

                currentmatch.controls = new List<Control>();

                int panelypos = masterpanel.VerticalScroll.Value;

                int y1pos = i * (gap + (2 * labh) + labg) + gap + (labh / 2) - panelypos;
                int y2pos = y1pos + labh + labg;
                int yvpos = y1pos + ((labh + labg) / 2);

                Label labv = new Label();
                labv.Font = new Font("Arial", 8, FontStyle.Regular);
                labv.Text = "v";
                labv.Location = new Point(frontxgap + xgap, yvpos);
                labv.AutoSize = true;
                masterpanel.Controls.Add(labv);
                currentmatch.controls.Add(labv);

                int scoreloc = maxlabsiz;

                TextBox score1 = new TextBox();
                score1.Font = new Font("Arial", 12, FontStyle.Regular);
                score1.Text = "";
                score1.Location = new Point(frontxgap + xgap + scoreloc, y1pos);

                score1.AutoSize = true;
                int scr1h = score1.Size.Height;
                score1.AutoSize = false;
                score1.Size = new Size(20, scr1h);

                masterpanel.Controls.Add(score1);
                currentmatch.controls.Add(score1);

                TextBox score2 = new TextBox();
                score2.Font = new Font("Arial", 12, FontStyle.Regular);
                score2.Text = "";
                score2.Location = new Point(frontxgap + xgap + scoreloc, y2pos);
                score2.AutoSize = false;
                score2.Size = new Size(20, scr1h);

                masterpanel.Controls.Add(score2);
                currentmatch.controls.Add(score2);

                currentmatch.p1txt = score1;
                currentmatch.p2txt = score2;

                currentround.matches[i] = currentmatch;
            }

            }

        private void updatematchscore()
         /* Recreate the match data whenever any of the text boxes are updated */

        {
            for (int i = 0; i < playerwins.Count; i++)
            {
                playerwins[i] = 0;
            }

            for (int i = 0; i < rounds.Count; i++)
            {
                for (int j = 0; j < rounds[i].matches.Count; j++)
                {
                    matchinfo currentmatch = rounds[i].matches[j];
                    currentmatch.finished = false;

                    if (!float.TryParse(currentmatch.p1txt.Text, out currentmatch.p1score))
                    {
                        currentmatch.p1score = 0;
                    }

                    if (!float.TryParse(currentmatch.p2txt.Text, out currentmatch.p2score))
                    {
                        currentmatch.p2score = 0;
                    }

                    if (currentmatch.p1score < currentmatch.p2score) // Player 2 win
                    {
                        playerwins[currentmatch.p2index] += 1;
                        currentmatch.finished = true;
                    }

                    else if (currentmatch.p1score > currentmatch.p2score) // Player 1 win
                    {
                        playerwins[currentmatch.p1index] += 1;
                        currentmatch.finished = true;
                    }

                    // Increment the amount opponent played for both players 

                    playerinfo pi = players[currentmatch.p1index];
                    pi.amountplayed[currentmatch.p2index]++;
                    players[currentmatch.p1index] = pi;

                    pi = players[currentmatch.p2index];
                    pi.amountplayed[currentmatch.p1index]++;
                    players[currentmatch.p2index] = pi;

                    // Add the match to the full match record

                    rounds[i].matches[j] = currentmatch;
                }

            }

        }

        private List<int> orderlist(List<float> lst) 
        {
            /* Produces an ordered list of currently playing players by wins, with players on the same win level randomly ordered.
             * 
             * List<float> lst: List of wins of each player with index corresponding to number to player id. 
             */

            List<float> score = new List<float>();
            List<int> index = new List<int>();

            List<int> nwlst = new List<int>();

            List<float> temp = new List<float>();

            List<int> tempindex = new List<int>();

            for (int i = 0; i < lst.Count; i++)
            {
                if (isplaying[i])
                {
                    tempindex.Add(i);
                    temp.Add(lst[i]);
                }
                
            }

            int next = 0;
            int cnt = temp.Count;

            for (int i = 0; i < cnt; i++) // Randomly orders the playing players, producing a corresponding list of players and scores
            {
                next = rnd.Next(0, temp.Count);
                score.Add(temp[next]);
                index.Add(tempindex[next]);
                temp.RemoveAt(next);
                tempindex.RemoveAt(next);
            }

            nwlst.Add(0);

            for (int i = 1; i < score.Count; i++)
            {
                int count = nwlst.Count;

                bool isinserted = false;

                for (int j = 0; j < count; j++)
                {

                    if (score[i] > score[nwlst[j]])
                    {
                        nwlst.Insert(j, i);

                        isinserted = true;
                        break;
                    }

                }

                if (!isinserted)
                    nwlst.Add(i);
            }

            List<int> nwind = new List<int>();

            for (int i = 0; i < nwlst.Count; i++)
            {
                nwind.Add(index[nwlst[i]]);
            }

            return nwind;
        }

        private List<int> orderlist2(List<float> lst)
        {
            List<int> temp = new List<int>();

            temp.Add(0);

            for (int i = 1; i < lst.Count; i++)
            {
                int count = temp.Count;

                bool isinserted = false;

                for (int j = 0; j < count; j++)
                {

                    if (lst[i] > lst[temp[j]])
                    {
                        temp.Insert(j, i);

                        isinserted = true;
                        break;
                    }

                }

                if (!isinserted)
                    temp.Add(i);
            }

            return temp;
        }
        private Dictionary<int, double> getWLPercentages(List<int> players) {
            Dictionary<int, double> playerpercents = new Dictionary<int, double>();
            

            Dictionary<int, int[]> playermatchdata = new Dictionary<int, int[]>(); // playerindex: [games, wins]
            foreach (int player in players)
            {
                playermatchdata[player] = new int[] {0, 0};
            }
            
            // Get number of games and wins for each player

            foreach (roundinfo round in rounds)
            {
                foreach(matchinfo match in round.matches)               // Iterate over each match in each round
                {
                    if (!match.finished)
                    {
                        continue;                                       // Match not finished; ignore it 
                    }
                    if (playermatchdata.ContainsKey(match.p1index)) {   // If player 1 is playing this round 
                        playermatchdata[match.p1index][0] += 1;         // Increment games by 1 for player 1
                        if (match.p1score > 0)                          // If player 1 wins
                        {
                            playermatchdata[match.p1index][1] += 1;     // Increment wins by 1
                        }
                    }

                    if (playermatchdata.ContainsKey(match.p2index))     // If player 2 is playing this round 
                    {   
                        playermatchdata[match.p2index][0] += 1;         // Increment games by 1 for player 2
                        if (match.p2score > 0)                          // If player 2 wins
                        {
                            playermatchdata[match.p2index][1] += 1;     // Increment wins by 2
                        }
                    }
                }
            }

            foreach(int playerindex in playermatchdata.Keys)
            {
                //Console.WriteLine(playermatchdata[playerindex][0].ToString() + ' ' + playermatchdata[playerindex][1].ToString());
                if (playermatchdata[playerindex][0] == 0)   // No games played, make percentage 0.5
                {
                    playerpercents[playerindex] = 0.5;
                }
                else
                {
                    playerpercents[playerindex] = (double)playermatchdata[playerindex][1] / (double)playermatchdata[playerindex][0];
                }
            }
           
            return playerpercents;
        }

        private List<int> getPlayingList()
        /* Returns a list containing the indexes of all currently ticked players */
        
        {
            List<int> players = new List<int>();
            for (int pindx=0; pindx < playerwins.Count; pindx++)
            {
                if (isplaying[pindx])
                {
                    players.Add(pindx);
                }
            }
            
            return players;

        }

        private int[,] generateMatchupMatrix(List<int> players)
        {
            int[,] matrix = new int[players.Count, players.Count];
            Dictionary<int, double> playerpercents = getWLPercentages(players);
            int minbyes = byes.Min();


            for (int i = 0; i< players.Count; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    if (i == j)
                    {
                        if (players.Count % 2 == 0)
                        {
                            matrix[i, j] = -1; // Even number of players, so bye not allowed 
                        } 
                        else
                        {
                            if (byes[players[i]] == minbyes) // Player is eligible for a bye
                            {
                                matrix[i, j] = 1000; // Bye allowed for player (high number prevents multiple byes being assigned in a single round)
                            }
                            else
                            {
                                matrix[i, j] = -1; // Player has already taken a bye in this cycle; not allowed
                            }
                            
                        }
                    }
                }
            }
        }
        private void generateRound()
        {
            List<int> players = getPlayingList();
            
        }
        private void createnewround()
        {
            generateRound();
            List<int> orderedplayers = orderlist(playerwins); // Produces ranked list of players by wins with players on the same wins randomly ranked
            

            bool done = false;
            int mastercounter = 0;

            bool done2 = false;
            int mastercounter2 = 0;

            roundinfo initround = new roundinfo();
            initround.matches = new List<matchinfo>();

            while (!done)
            {

                mastercounter++;

                if (mastercounter > 1000)
                {
                    done = true;
                    string message = "while loop unexpected exit";
                    string caption = "";
                    MessageBox.Show(message, caption, MessageBoxButtons.OK);
                }

                if (orderedplayers.Count == 0)
                {
                    done = true;
                    break;
                }

                int currentplayerindex = orderedplayers[0];
                orderedplayers.RemoveAt(0);

                int targetplays = 0;

                done2 = false;

                while (!done2)
                {

                    mastercounter2++;

                    if (mastercounter2 > 1000000)
                    {
                        done2 = true;
                        string message = "while loop unexpected exit";
                        string caption = "";
                        MessageBox.Show(message, caption, MessageBoxButtons.OK);
                    }

                    for (int i = 0; i < orderedplayers.Count; i++)
                    {

                        if (players[currentplayerindex].amountplayed[orderedplayers[i]] == targetplays)
                        {
                            matchinfo match = new matchinfo();
                            match.p1index = currentplayerindex;
                            match.p2index = orderedplayers[i];
                            initround.matches.Add(match);

                            orderedplayers.RemoveAt(i);


                            done2 = true;

                            break;
                        }

                    }

                    targetplays++;
                }

            }

            rounds.Add(initround);
        }


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //endroundbutton
        {
            updateplayerlist();
            updatematchscore();
            createnewround();
            updateUI(rounds.Count - 1);
        }

        private void updateplayerlist()
        {
            List<string> selectedplayers = form1.getplayerlist_names();
            List<float> selectedrankings = form1.getplayerlist_rank();

            List<string> rmplayers = new List<string>();
            List<float> rmrankings = new List<float>();

            for (int i = 0; i < selectedplayers.Count; i ++)
            {
                bool isnew = true;
                
                for(int j = 0; j < playernames.Count; j ++)
                {
                    if (selectedplayers[i] == playernames[j])
                    {
                        isnew = false;
                        isplaying[j] = true;
                    }
                }

                if (isnew)
                {
                    for(int j = 0; j < players.Count; j++)
                    {
                        players[j].amountplayed.Add(0);
                    }
                    
                    playernames.Add(selectedplayers[i]);
                    playerrankings.Add(selectedrankings[i]);
                    isplaying.Add(true);
                    playerwins.Add(0);

                    playerinfo pi = new playerinfo();

                    pi.amountplayed = newlist<int>(playernames.Count, 0);

                    players.Add(pi);
                }
            }



            for (int i = 0; i < playernames.Count; i++)
            {
                bool rem = true;

                for (int j = 0; j < selectedplayers.Count; j++)
                {
                    if (playernames[i] == selectedplayers[j])
                    {
                        rem = false;
                    }
                }

                if (rem)
                {
                    isplaying[i] = false;
                }
            }


        }

        private void endweek_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();

            /*string message = "Would you like to end the week?";
            string caption = "Attention";

            if(MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                /*updatematchscore();
                returnmatchdata();
                updaterankings();
                returnrankings();
               

                string path = Directory.GetCurrentDirectory();
                path += "\\Resources";
                //if(!Directory.Exists(path))
                path = Directory.GetParent(Form1._rp).FullName;

                DateTime current = DateTime.Now;

                string file = path + "\\rankings_" + current.Year + "." + current.Month + "." + current.Day + ".txt";

                var process = System.Diagnostics.Process.Start(file);
                process.WaitForInputIdle();
                    }*/
        }

        private void updaterankings()
        {

            for (int i = 0; i < playerwins.Count; i++)
            {
                playerwins[i] = 0;
            }

            for (int i = 0; i < rounds.Count; i++)
            {

                calculaterankings(i);

            }

            for (int i = 0; i < playerrankings.Count; i++)
            {
                playerrankings[i]++;
            }

        }

        private void calculaterankings(int roundnumber)
        {

            for (int j = 0; j < rounds[roundnumber].matches.Count; j++)
            {
                matchinfo currentmatch = rounds[roundnumber].matches[j];

                float rankingplus = 1;

                if (currentmatch.p1score > currentmatch.p2score)
                {
                    float rankdif = playerrankings[currentmatch.p2index] - playerrankings[currentmatch.p1index];

                    if (rankdif > 0)
                    {
                        rankingplus += (rankdif / 5);
                    }

                    playerrankings[currentmatch.p1index] += rankingplus;
                }

                if (currentmatch.p1score < currentmatch.p2score)
                {
                    float rankdif = playerrankings[currentmatch.p1index] - playerrankings[currentmatch.p2index];

                    if (rankdif > 0)
                    {
                        rankingplus += (rankdif / 5);
                    }

                    playerrankings[currentmatch.p2index] += rankingplus;
                }

            }

        }

        private void returnrankings()
        {
            List<string> fullplay = playernames;
            List<float> fullrank = playerrankings;

            for(int i = 0; i < fullplay.Count; i++)
            {
                if(fullplay[i]== "Bye")
                {
                    fullplay.RemoveAt(i);
                    fullrank.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < fullplayernames.Count; i++)
            {
                
                if(!fullplay.Contains(fullplayernames[i]))
                {
                    fullplay.Add(fullplayernames[i]);
                    fullrank.Add(fullplayerrankings[i]);

                }

            }

            List<int> orderedrankings = orderlist2(fullrank);

            string path = Directory.GetCurrentDirectory();
            path += "\\Resources";
            //if(!Directory.Exists(path))
            path = Directory.GetParent(Form1._rp).FullName;

            DateTime current = DateTime.Now;

            string file = path + "\\rankings_" + current.Year + "." + current.Month + "." + current.Day + ".txt";

            StreamWriter sw = new StreamWriter(file);


            for (int i = 0; i < orderedrankings.Count; i ++)
            {
                sw.WriteLine(fullplay[orderedrankings[i]] + "," + fullrank[orderedrankings[i]]);
            }

            sw.Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

                string message = "Would you like to end the week?";
                string caption = "Attention";

                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    updatematchscore();
                    returnmatchdata();
                    updaterankings();
                    returnrankings();


                    string path = Directory.GetCurrentDirectory();
                    path += "\\Resources";
                    //if(!Directory.Exists(path))
                    path = Directory.GetParent(Form1._rp).FullName;

                    DateTime current = DateTime.Now;

                    string file = path + "\\rankings_" + current.Year + "." + current.Month + "." + current.Day + ".txt";

                    System.Diagnostics.Process.Start(file);

                }

                else
                {
                    e.Cancel = true;
                }

        }
    
        private void returnmatchdata()
        {
            string path = Directory.GetCurrentDirectory();
            path += "\\Resources";
            //if(!Directory.Exists(path))
            path = Directory.GetParent(Form1._rp).FullName;

            DateTime current = DateTime.Now;

            string file = path + "\\matches_" + current.Year + "." + current.Month + "." + current.Day + ".txt";

            StreamWriter sw = new StreamWriter(file);

            for (int i = 0; i < rounds.Count; i++)
            {
                sw.WriteLine("Round " + (i+1));

                string line1 = "";
                string line2 = "";
                string line3 = "";

                for (int j = 0; j < rounds[i].matches.Count; j++)
                {
                    matchinfo currentmatch = rounds[i].matches[j];
                    line1 += "Match " + (j+1) + ",,,";
                    line2 += playernames[currentmatch.p1index] + ", " + currentmatch.p1score + ",,";
                    line3 += playernames[currentmatch.p2index] + ", " + currentmatch.p2score + ",,";

                }
                sw.WriteLine(line1);
                sw.WriteLine(line2);
                sw.WriteLine(line3);

                sw.WriteLine("");
            }

            sw.Close();
        }

        private void Form2_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {

            resize_panel();

        }

        private void resize_panel ()
        {

            int topgap = masterpanel.Location.Y;
            int gap = masterpanel.Location.X;
            int height = this.Size.Height - (topgap + (4 * gap));
            int width = this.Size.Width - ( 3 * gap);

            masterpanel.Size = new Size(width, height);

        }

        private void masterpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    
}
