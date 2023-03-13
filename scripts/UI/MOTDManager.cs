using Godot;
using System;

public partial class MOTDManager : Label {
	string[] MotdMessages;
	
	public override async void _Ready() {
		MotdMessages = new string[] {
			"▇▅▆▇▆▅▅█",
			"pro tip: shoot yourself",
			"you should recoil yourself... NOW!!",
			"1 in 10 people reportedly are living",
			"why do they call it a gun no run when you run of the gun no out gun of run",
			"the missile knows where it is because it knows where it isnt",
			"he is coming. thats disgusting",
			"id like to make an announcement", 
			"I | Ii | II | L",
			"this game is d",
			"this game is r",
			"run",
			"Perfectly Balanced",
			"i dont invoke the fifth amendment i will not shut up",
			"le fishe au chocolat",
			"play less",
			OS.GetName() == "Windows" ? "...but your crimes against linuxkind... will NOT be forgotten. And thy punishment... is DEATH." : (DateTime.Now.Year + 1) + " will be the year of the linux desktop",
			"google en passant",
			"holy hell",
			"im too busy ???? my gender", 
			"your mom straight",
			"i fucked your dad... financially. he will never recover",
			"i am in your walls.",
			"you will shit your pants you will shit your pants you will shit your pa",
			":]",
			"i gazed upon god, and he was weeping",
			"kel got that lego smile",
			"AAAAAAAAAAAAAAAAAAAAAAAAAAHHHH",
			"naaah no wayyy omg bruuuhhh",
			"you call flipping a coin a martial art?",
			"The FitnessGram Pacer test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter Pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal *boop*. A single lap should be completed each time you hear this sound *ding*. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.",

		};

		var rnd = new RandomNumberGenerator();
		Text = MotdMessages[rnd.RandiRange(0, MotdMessages.Length - 1)];

		if (Text == "▇▅▆▇▆▅▅█") {
			LabelSettings.FontColor = new Color(170, 0, 0, 1);
		} else if (Text == "i am in your walls.") {
			await this.Sleep(2.5f);
			GetNode<AudioStreamPlayer>("../DoorKnock").Play();
		} else if (Text == "naaah no wayyy omg bruuuhhh") {
			await this.Sleep(0.5f);
			GetNode<AudioStreamPlayer>("../VineBoom").Play();
		} else if (Text == "The FitnessGram Pacer test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter Pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal *boop*. A single lap should be completed each time you hear this sound *ding*. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.") {
			LabelSettings.FontSize = 8;
		}
	}
}
