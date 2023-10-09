using System;
using Godot;

public partial class MOTDManager : Label {
	string[] MotdMessages;
	
	public override async void _Ready() {
		LabelSettings.FontSize = 50;
		LabelSettings.FontColor = new Color(255, 255, 255, 1);

		MotdMessages = new string[] {
			"▇▅▆▇▆▅▅█",
			"pro tip: shoot yourself",
			"you should recoil yourself... NOW!!",
			"1 in 10 people reportedly are living",
			"why do they call it a gun no run when you run of the gun no out gun of run",
			"the missile knows where it is because it knows where it isnt",
			"HE is coming.",
			"he is coming. thats disgusting",
			"he is coming. thats hot",
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
			"1.048596",
			"El. Psy. Kongroo.",
			"beta messenger vs the alpha gate of steiner",
			"Official home of the Alzheimers Support Group",
			"Unlimited Gun Works",
			"A robot picked up a gun. This is what happened to their legs.",
			"KILL WARDENS BURN WARDENS ALIVE SMASH WARDEN MORALE TURN WARDENS INTO PASTE",
			"MAKE SOYDAWG BOOT UP MICROSOFT PAINT",
			"Live Gun Reaction",
			"And then, Hefest got this run...",
			"do people die when they are killed?",
			"apt fucking sucks",
			"hello spez",
			"tights :3",
			"https://play.meltyblood.club",
			"A las barricadas!",
			"she is coming. mmmff,,,",
			"ぬるぽ",
			"が",
			"you know what? fuck you. *crashes your game*",
			"Home of CHALLENGE PISSING",
			"transing your kids gender since 2013",
			"help my piss is on fire",
			"Also play Terraria!",
			"hey so hows your day",
			"Bruno Powroznik approved",
			"im not gonna sugarcoat it. 5A 5B 2B 2C 3C j.ABC dj.ABC AT",
			"you should return the slab... NOW!!",
			" ",
			"Which side are you on?",
			"One must imagine robot happy",
			"quila was here",
			"mulch is heeeeeeeere",
			"webdev is scuffed",
			"This will be gaming in 2015",
			"gerg",
			"greg",
			"greg heffley huffling helium whole",
			"gerg heffley huffling helium whole",
			"goon",
			"KILL CONSUME MULTIPLY CONQUER",
			"Haiiii :3",
			"dev bias",
			"movie coming in 2057.63"
		};

		var rnd = new RandomNumberGenerator();
		Text = MotdMessages[rnd.RandiRange(0, MotdMessages.Length - 1)];
		//Text = MotdMessages[MotdMessages.Length - 1];

		if (Text == "▇▅▆▇▆▅▅█") {
			LabelSettings.FontColor = new Color(170, 0, 0, 1);
		} else if (Text == "i am in your walls.") {
			await this.Sleep(2.5f);
			GetNode<AudioStreamPlayer>("DoorKnock").Play();
		} else if (Text == "naaah no wayyy omg bruuuhhh") {
			await this.Sleep(0.5f);
			GetNode<AudioStreamPlayer>("VineBoom").Play();
		} else if (Text == "The FitnessGram Pacer test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter Pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal *boop*. A single lap should be completed each time you hear this sound *ding*. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.") {
			LabelSettings.FontSize = 8;
		} else if (Text == "hello spez") {
			GetNode<TextureRect>("Knight").Show();
		} else if (Text == "you know what? fuck you. *crashes your game*") {
			await this.Sleep(2f);
			GetTree().Root.GuiEmbedSubwindows = false;
			
			var dialog = new AcceptDialog();
			dialog.DialogText = "nah lol";
			GetTree().Root.AddChild(dialog);
			dialog.PopupCentered();

			dialog.Confirmed += () => dialog.QueueFree();
			dialog.Canceled += () => dialog.QueueFree();
			dialog.TreeExited += () => GetTree().Root.GuiEmbedSubwindows = true;

		} else if (Text == " ") {
			GetNode<TextureRect>("LiveLeak").Show();
		} else if (Text == "quila was here") {
			LabelSettings.FontColor = new Color("1c1c1c");
		}
	}
}
