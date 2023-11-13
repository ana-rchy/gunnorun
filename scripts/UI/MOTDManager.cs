using System;
using System.Threading.Tasks;
using Godot;

public partial class MOTDManager : RichTextLabel {
	[Export] AudioStreamPlayer _doorKnock;
	[Export] AudioStreamPlayer _vineBoom;
	[Export] AudioStreamPlayer _navalInvasion;
	[Export] TextureRect _unhingedKnight;
	[Export] TextureRect _liveLeak;
	
	string[] _motdMessages;
	
	public override async void _Ready() {
		AddThemeFontSizeOverride("normal_font_size", 50);
		AddThemeFontSizeOverride("italics_font_size", 50);
		RenderingServer.SetDefaultClearColor(new Color("1e1e1e"));

		_motdMessages = new string[] {
			"[color=#aa0000]▇▅▆▇▆▅▅█[/color]",
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
			OS.GetName() == "Windows" ? "...but your crimes against linuxkind... will NOT be forgotten. And thy punishment... is DEATH." : $"{DateTime.Now.Year + 1} will be the year of the linux desktop",
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
			"[color=#1c1c1c]quila was here[/color]",
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
			"drink water or death",
			"HYDRATE MOTHERFUCKER",
			"when you at the when you you when the",
			"WHEN HE, WHEN HE AT THE",
			"[color=#0cff04]CAVERN LIGHT SEVERED\nYOU ARE A GUN AUTOMATON ANIMATED BY NEUROTRANSMITTERS[/color]",
			"hhhhgffjffgjgjhgfhgfhgfdgdfggffffhhhffffhfhhhhhh",
			"You should drink water... NOW!!",
			"10",
			"QRF the baths",
			"jack, youre gonna get offline raided on rustafied EU small tomorrow",
			"no state solution",
			"There will be blood!",
			"Inside your guns there are things you can't see... it's something that [i]\"does not exist\"[/i] in this world...",
			"GUN NO RUN: GO BEYOND",
			"GUN ACT 4",
			"[color=#aa0000]Dangerous naval invasion![/color]",
			"A GUN DOES NOT FEAR DEATH!",
			"It's not over yet, dik.",
			"youre next in the flow of calamity",
			"Are you... [i]pursuing[/i] me?",
			"Now with more french bread!",
			"im in spain, but with the s",
			"I FUCKING HATE NANAYAAAAAAAAAAAAAAAAAAAAAAAAA",
			"「MADE IN HEAVEN」"
		};

		var rnd = new RandomNumberGenerator();
		var uncenteredText = Text = _motdMessages[rnd.RandiRange(0, _motdMessages.Length - 1)];
		//Text = MotdMessages[MotdMessages.Length - 1];

		Text = $"[center]{Text}[/center]";
		CallDeferred(nameof(ResizeText));
		
		try {
			switch (uncenteredText) {
				case "i am in your walls.":
					await this.Sleep(2.5f);
					_doorKnock.Play(); break;
				case "naaah no wayyy omg bruuuhhh":
					await this.Sleep(0.5f);
					_vineBoom.Play(); break;
				case "The FitnessGram Pacer test is a multistage aerobic capacity test that progressively gets more difficult as it continues. The 20 meter Pacer test will begin in 30 seconds. Line up at the start. The running speed starts slowly, but gets faster each minute after you hear this signal *boop*. A single lap should be completed each time you hear this sound *ding*. Remember to run in a straight line, and run as long as possible. The second time you fail to complete a lap before the sound, your test is over. The test will begin on the word start. On your mark, get ready, start.":
					AddThemeFontSizeOverride("normal_font_size", 8); break;
				case "hello spez":
					_unhingedKnight.Show(); break;
				case "you know what? fuck you. *crashes your game*":
					await this.Sleep(2f);
					GetTree().Root.GuiEmbedSubwindows = false;

					var dialog = new AcceptDialog();
					dialog.DialogText = "idiot";
					GetTree().Root.AddChild(dialog);
					dialog.PopupCentered();

					dialog.Confirmed += () => dialog.QueueFree();
					dialog.Canceled += () => dialog.QueueFree();
					dialog.TreeExited += () => GetTree().Root.GuiEmbedSubwindows = true; break;
				case " ":
					_liveLeak.Show(); break;
				case "[color=#0cff04]CAVERN LIGHT SEVERED\nYOU ARE A GUN AUTOMATON ANIMATED BY NEUROTRANSMITTERS[/color]":
					RenderingServer.SetDefaultClearColor(new Color("000000")); break;
				case "10":
					_ = Countdown(uncenteredText); break;
				case "[color=#aa0000]Dangerous naval invasion![/color]":
					_navalInvasion.Play(); break;
			}
		} catch (Exception e) {
			if (e is ObjectDisposedException) {
				GD.Print("bad object dispose :(");
			} else {
                throw;
            }
		}
	}

    public override void _ExitTree() {
        RenderingServer.SetDefaultClearColor(new Color("1e1e1e"));
    }

	//---------------------------------------------------------------------------------//
    #region | funcs

	void ResizeText() {
		if (Size.X > 1900) {
			var fontSize = (int) (-0.021 * (Size.X - 1900)) + 50;
			AddThemeFontSizeOverride("normal_font_size", fontSize); // desmos that shit
			AddThemeFontSizeOverride("italics_font_size", fontSize);
		}
	}

	async Task Countdown(string uncenteredText) {
		var i = int.Parse(uncenteredText);
		for (;; i--) {
			SetDeferred("text", $"[center]{i}[/center]");
			await this.Sleep(1f);
		}
	}

	#endregion
}
