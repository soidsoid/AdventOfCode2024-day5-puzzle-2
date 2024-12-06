var directory = Directory.GetCurrentDirectory();
var instructionsFile = File.ReadAllLines($"{directory}\\..\\..\\..\\instructions.txt");
var rulesFile = File.ReadAllLines($"{directory}\\..\\..\\..\\rules.txt");

Updates updates = new Updates(instructionsFile);
var ruleSet = new List<Rule>();
foreach(var line in rulesFile)
{
    var rule = new Rule
    {
        First = int.Parse(line.Substring(0, 2)),
        Second = int.Parse(line.Substring(3, 2))
    };
    ruleSet.Add(rule);
}

foreach(var rule in ruleSet)
{
    Console.WriteLine($"{rule.First}|{rule.Second}");
}

var middleValues = new List<int>();
foreach (var update in updates.Instructions)
{
    for (int i = 0; i < update.Length; i++)
    {
        Console.Write($"{update[i]}");
        if (i < update.Length - 1)
            Console.Write(",");
    }
    var valid = IsInstructionSetValid(update, ruleSet);
    Console.Write($" instruction valid?: {valid}");

    if(valid)
    {
        var middle = (int)Math.Ceiling((double)update.Length / 2) - 1;
        middleValues.Add(update[middle]);
    }

    Console.WriteLine();
}

Console.WriteLine($"Middle value sum: {middleValues.Sum()}");

static bool IsInstructionSetValid(int[] instructions, List<Rule> rules)
{

    var relevantRules = new List<Rule>();

    foreach(var rule in rules)
    {
        if(instructions.Contains(rule.First) || instructions.Contains(rule.Second))
        {
            relevantRules.Add(rule);
        }
    }

    var ruleSet = new RuleSet(relevantRules);

    List<int> correctOrder = new List<int>();

    for(int i = 0; i < instructions.Length; i++)
    {
        if(i == 0)
        {
            correctOrder.Add(instructions[i]);
            continue;
        }

        if (ruleSet.ContainsInstruction(instructions[i]))
        {
            var previousInstructions = correctOrder[0..i];
            if(ruleSet.CanIAddNewInstruction(previousInstructions, instructions[i]))
            {
                correctOrder.Add(instructions[i]);
            }
            else
            {
                return false;
            }
        }
        else
        {
            correctOrder.Add(instructions[i]);
        }
    }
    return true;
}

class RuleSet
{
    public List<Rule> Rules { get; set; }

    public RuleSet(List<Rule> rules)
    {
        Rules = rules;
    }

    public bool ContainsInstruction(int instruction)
    {
        foreach(var rule in Rules)
        {
            if(rule.ContainsInstruction(instruction))
                return true;
        }
        return false;
    }

    public bool CanIAddNewInstruction(List<int> previousInstruction, int newInstruction)
    {

        var relevantRules = Rules.Where(x => x.ContainsInstruction(newInstruction)).ToList();

        foreach(var instruction in previousInstruction)
        {
            if(relevantRules.Where(x => x.MatchFirstAndSecond(newInstruction, instruction)).Any())
            {
                return false;
            }
        }
        return true;
    }
}

class Rule
{
    public int First { get; set; }
    public int Second { get; set; }

    public bool ContainsInstruction(int instruction) => instruction == First || instruction == Second;

    public bool MatchFirstAndSecond(int first, int second) => first == First && second == Second;
}

class Updates
{
    public List<int[]> Instructions { get; set; } = new List<int[]>();

    public Updates(string[] instructions)
    {
        foreach(var line in instructions)
        {
            var update = line.Split(",")
                .Select(x => int.Parse(x)).ToArray();

            Instructions.Add(update);
        }
    }

    public int Middle => (int)Math.Ceiling((double)Instructions.Count / 2);
}
