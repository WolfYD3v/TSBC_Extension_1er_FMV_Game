using Godot;
using System;

public partial class Terminal : Control
{
	[Signal]
	public delegate void S_NextVideoSceneEventHandler();

	[Export]
	string V_UserName = "leon";
	[Export]
	string V_DeviceName = "Lenovo-G70-70";
	int V_MinimumTextLenght = 0;

	bool V_AllowTextInput = false;

	RichTextLabel N_TerminalText;
	string V_Instruction = "";

	[Export]
	Godot.Collections.Dictionary V_NeoBashCommands = new Godot.Collections.Dictionary
	{
		{
			"buggy-sims-board-api", new Godot.Collections.Dictionary {
				{ "connect", "F_StartFMV" }
			}
		},
		{
			"sims-board-api-connect", "F_StartFMV"
		},
		{
			"help", "F_TerminalHelp"
		},
		{
			"whoami", "F_WhoAmI"
		},
		{
			"clear", "F_Clear"
		}
	};

	public async override void _Ready()
	{
		// Get user name of the session, try at least
		if (OS.HasEnvironment("USERNAME"))
		{
			GD.Print("USERNAME");
			V_UserName = OS.GetEnvironment("USERNAME");
		}

		Tween V_Tween = GetTree().CreateTween();
		V_Tween.TweenProperty(this, "size:y", 648.0f, 5.0f);
		await ToSignal(V_Tween, "finished");
		V_Tween.Kill();

		N_TerminalText = GetNode<RichTextLabel>("TerminalRichTextLabel");

		N_TerminalText.Text = "Bienvenue Player096!" + "\n";
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		N_TerminalText.Text += "[NeoBash 0.54.7-dev-build]" + "\n" + "\n";
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		N_TerminalText.Text += "Ecrire 'help' pour avoir de l'aide, logique.";
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		F_AddLineInTerminal();
	}

	// Gemini used here!!! (https://gemini.google.com/share/0d16214aa8ac)
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (V_AllowTextInput == false)
		{
			return;
		}

		if (@event is InputEventKey V_InputEventKey)
		{
			if (V_InputEventKey.Pressed)
			{
				if (V_InputEventKey.IsActionPressed("TerminalEnterKeyPressed"))
				{
					GD.Print("ENTER PRESSED!");
					F_ProcessInstruction();
				}
				else
				{
					long V_UnicodeValue = V_InputEventKey.Unicode;
					F_TreatInput(V_UnicodeValue);
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void F_TreatInput(long V_UnicodeValue)
	{
		if (V_UnicodeValue > 0)
		{
			char V_PressedChar = (char)V_UnicodeValue;

			N_TerminalText.Text += V_PressedChar;
			V_Instruction += V_PressedChar;

			GetViewport().SetInputAsHandled();
		}
		else if (V_UnicodeValue == 0)
		{
			if (V_Instruction.Length - 1 >= 0)
			{
				N_TerminalText.Text = N_TerminalText.Text.Remove(N_TerminalText.Text.Length - 1, 1);
				V_Instruction = V_Instruction.Remove(V_Instruction.Length - 1, 1);
			}
		}
	}

	public void F_AddLineInTerminal(string V_Text = "")
	{
		V_AllowTextInput = false;
		N_TerminalText.Text += "\n";
		N_TerminalText.Text += $"[color=green]{V_UserName}@{V_UserName}-{V_DeviceName}:[/color]~$ ";
		V_MinimumTextLenght = N_TerminalText.Text.Length;
		N_TerminalText.Text += V_Text;
		V_AllowTextInput = true;
	}

	private async void F_ProcessInstruction()
	{
		V_AllowTextInput = false;

		string[] V_InstructionWords = F_InstructionsToWords();
		GD.Print($"Mots extraits : {string.Join(", ", V_InstructionWords)}");

		Godot.Collections.Dictionary V_CommandSection = V_NeoBashCommands.Duplicate();
		foreach (string V_InstructionWord in V_InstructionWords)
		{
			if (V_CommandSection.ContainsKey(V_InstructionWord))
			{
				GD.Print(V_InstructionWord);

				var V_JSPNom = V_CommandSection[V_InstructionWord];

				// 1. **PRIORITÉ** : Vérification de la String
				string stringValue = V_JSPNom.As<string>();

				if (stringValue != null && stringValue.Length > 0)
				{
					// CAS 1 : C'est une Chaîne de caractères (commande à exécuter).
					GD.Print("Call!");
					CallDeferred(stringValue);
				}
				else
				{
					// 2. Vérification du Dictionnaire : Tente de convertir la Variant en Dictionary.
					Godot.Collections.Dictionary dictionaryValue = V_JSPNom.As<Godot.Collections.Dictionary>();

					if (dictionaryValue != null)
					{
						// CAS 2 : C'est un Dictionnaire (navigation dans la structure de commande).
						GD.Print("Change!");
						V_CommandSection = dictionaryValue;
					}
					else
					{
						// CAS 3 : Ni l'un, ni l'autre. Erreur ou type inattendu.
						GD.PushError($"Type de commande non géré pour : {V_JSPNom.GetType()}");
						F_SendOutput("[color=red]ERROR:[/color] Type de commande inattendu.");
					}
				}
			}
			else
			{
				F_SendOutput("[color=red]ERROR:[/color] Commande non reconnue.");
			}
		}

		V_Instruction = "";

		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		F_AddLineInTerminal();

		V_AllowTextInput = true;
	}

	private string[] F_InstructionsToWords()
	{
		// La méthode Split() divise la chaîne par le ou les délimiteurs spécifiés.
		// L'argument StringSplitOptions.RemoveEmptyEntries garantit que les espaces multiples
		// ne créent pas d'entrées vides dans le tableau.
		string[] V_InstructionWords = V_Instruction.Split(
			new char[] { ' ' },
			StringSplitOptions.RemoveEmptyEntries
		);

		return V_InstructionWords;
	}

	private void F_SendOutput(string V_OutputText)
	{
		N_TerminalText.Text += "\n";
		N_TerminalText.Text += V_OutputText;
	}

	// Fs
	public void F_TerminalHelp()
	{
		F_SendOutput("sims-board-api-connect -> Se connecter au Sims Board");
		F_SendOutput("whoami -> Retourne le nom d'utilisateur");
		F_SendOutput("clear -> Efface pour de vrai le texte dans le terminal, pas comme dans les autres...");
	}
	public async void F_StartFMV()
	{
		Tween V_Tween = GetTree().CreateTween();
		V_Tween.TweenProperty(this, "size:y", 0.0f, 3.5f);

		F_SendOutput("Connection en cours au Sims Board...");
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		F_SendOutput("Connection au serveur 1063...");
		await ToSignal(GetTree().CreateTimer(0.3f), "timeout");

		await ToSignal(V_Tween, "finished");
		V_Tween.Kill();

		EmitSignal(SignalName.S_NextVideoScene);
	}
	public void F_WhoAmI()
	{
		F_SendOutput(V_UserName);
	}
	public void F_Clear()
	{
		N_TerminalText.Text = "";
	}
}
