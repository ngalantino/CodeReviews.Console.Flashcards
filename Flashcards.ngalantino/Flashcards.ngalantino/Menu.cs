using System.Data.Common;
using Spectre.Console;
public static class Menu
{
    private static string mainMenuChoice = "";
    public static string selectedStack = "No stack selected";
    public static void DisplayMenu()
    {

        while (mainMenuChoice != "Exit")
        {
            Console.Clear();
            // Create main prompt
            mainMenuChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Select an option")
            .PageSize(10)
            .MoreChoicesText("(Move up and down to reveal more options)")
            .AddChoices(new[] {
                "Exit",
                "Manage stacks",
                "Manage flashcards",
                "Study",
                "View study session data"
            }));

            switch (mainMenuChoice.ToLower())
            {
                case "manage stacks":

                    while (mainMenuChoice != "Return to main menu")
                    {
                        Console.Clear();

                        mainMenuChoice = StudyContentController.ManageStacks();

                        switch (mainMenuChoice.ToLower())
                        {

                            case "change current stack":
                                selectedStack = StudyContentController.SelectStack();
                                break;

                            case "view all stacks":
                                List<Stack> stacks = StudyContentController.GetStacks();

                                DisplayTable(stacks);

                                Console.WriteLine("Press enter to continue...");

                                Console.ReadLine();

                                break;

                            case "create a new stack":
                                StudyContentController.NewStack();
                                break;

                            case "delete a stack":
                                String stackToDelete = StudyContentController.SelectStack();

                                StudyContentController.DeleteStack(stackToDelete);
                                break;
                        }

                    }

                    break;

                case "manage flashcards":
                    while (mainMenuChoice != "Return to main menu")
                    {

                        mainMenuChoice = StudyContentController.ManageFlashcards();

                        switch (mainMenuChoice)
                        {
                            case "Return to main menu":

                                break;

                            case "Change current stack":
                                selectedStack = StudyContentController.SelectStack();
                                break;

                            case "View all flashcards in stack":
                                Console.Clear();
                                List<Flashcard> flashcards = StudyContentController.GetFlashcardsInStack();

                                DisplayTable(flashcards);

                                break;

                            case "View X amount of cards in stack":
                                Console.WriteLine("How many flashcards do you want to view?");
                                int numFlashcards = Int32.Parse(Console.ReadLine());
                                List<Flashcard> xAmountofFlashcards = StudyContentController.GetFlashcardsInStack(numFlashcards);

                                DisplayTable(xAmountofFlashcards);

                                break;

                            case "Create a flashcard in current stack":
                                StudyContentController.CreateFlashcard();
                                break;

                            case "Edit a flashcard":

                                StudyContentController.EditFlashcard();
                                break;

                            case "Delete a flashcard":
                                StudyContentController.DeleteFlashcard();
                                break;

                            default:
                                break;
                        }
                    }

                    break;

                case "study":
                    // Select stack
                    if (selectedStack == "No stack selected")
                    {
                        selectedStack = StudyContentController.SelectStack();
                        Console.Clear();
                    }

                    // If selected stack has no flashcards, instruct user to add flashcards first.
                    if (StudyContentController.GetFlashcardsInStack().Count == 0)
                    {
                        Console.WriteLine("No flashcards in stack!");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        break;
                    }


                    // Begin study session by getting first flashcard in stack and iterating
                    List<Flashcard> studyFlashcards = StudyContentController.GetFlashcardsInStack();

                    string answer = "";
                    int score = 0;
                    DateTime dateTime = DateTime.Now;
                    string dateString = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
                    // Store dateString in the database


                    while (answer != "0")
                    {
                        foreach (Flashcard flashcard in studyFlashcards)
                        {
                            Console.WriteLine(flashcard.Front);
                            Console.WriteLine("Input your answer or press 0 to exit.");
                            answer = Console.ReadLine();

                            if (answer == "0")
                            {
                                break;
                            }

                            // Check answer
                            if (answer.ToLower().Equals(flashcard.Back.ToLower()))
                            {
                                Console.WriteLine("Correct!");
                                score++;
                            }
                            else
                            {
                                Console.WriteLine("Wrong!");
                            }
                        }
                    }
                    StudyContentController.NewStudySession(selectedStack, dateString, score);

                    break;

                case "view study session data":

                    DisplayTable(StudyContentController.GetStudySessions());
                    Console.WriteLine("Press any key to return.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    public static void DisplayTable(List<Flashcard> flashcards)
    {
        // Display table with ID, front, and back
        Table table = new Table();

        table.AddColumn("Id");
        table.AddColumn("Front");
        table.AddColumn("Back");

        int id = 1;

        foreach (Flashcard flashcard in flashcards)
        {
            table.AddRow(id.ToString(), flashcard.Front, flashcard.Back);
            id++;
        }

        AnsiConsole.Write(table);
    }

    public static void DisplayTable(List<StudySession> studySessions)
    {
        Table table = new Table();

        table.AddColumn("Stack");
        table.AddColumn("Date");
        table.AddColumn("Score");

        foreach (StudySession session in studySessions)
        {
            table.AddRow(session.Stack, session.Date.ToString(), session.Score.ToString());
        }

        AnsiConsole.Write(table);
    }

    public static void DisplayTable(List<Stack> stacks)
    {

        Table table = new Table();

        table.AddColumn("Stacks");

        foreach (Stack stack in stacks)
        {
            table.AddRow(stack.Name);
        }

        AnsiConsole.Write(table);
    }
}