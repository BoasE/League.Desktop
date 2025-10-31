# Test Structure Documentation

## Overview

This documentation describes the test structure, naming conventions, and tools used in our projects. It serves as a reference for creating consistent unit tests across different domains and projects.

**Originally based on**: BE.Learning project  
**Extended with**: BE.League.Desktop project patterns

---

## 1. Test Tools & Frameworks

### NuGet Packages
```xml
<PackageReference Include="xunit" Version="2.9.2"/>
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2"/>
<PackageReference Include="FakeItEasy" Version="8.3.0" />
<PackageReference Include="coverlet.collector" Version="6.0.2"/>
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1"/>
```

### Tool Overview
- **xUnit**: Test framework (primary framework)
- **FakeItEasy**: Mocking framework
- **Coverlet**: Code coverage tool
- **Microsoft.NET.Test.Sdk**: Test SDK for .NET

---

## 2. Naming Conventions

### 2.1 Test Class Names

**Pattern**: `Given{ClassUnderTest}` or `When{Scenario}`

#### Examples:
```csharp
// Base class for component tests
public class GivenWorksheet { }
public class GivenLetterRecognitionWorksheetDomainObject { }
public class GivenTaskCulture { }
public class GivenDsl { }

// From BE.League.Desktop:
public class GivenLiveClientObjectReader { }
public class GivenChampSelectSession { }
public class GivenLobbyMember { }

// Scenario-based tests (often as nested tests)
public sealed class WhenCreated : GivenLearnplanStateFact { }
public sealed class WhenLearnPlanCreated : GivenLearnplanStateFact { }
public sealed class WhenCreatedAndManuallyAddedToPlanned : GivenLearnplanStateFact { }

// From BE.League.Desktop:
public sealed class WhenDeserializingAllGameData : GivenLiveClientObjectReader { }
public sealed class WhenDeserializingActivePlayer : GivenLiveClientObjectReader { }
public sealed class WhenDeserializingLobby : GivenLiveClientObjectReader { }
```

### 2.2 Test Method Names

**Pattern**: 
- `It{DescribesExpectedBehavior}` for simple assertions
- `{Action}_{Condition}_{ExpectedResult}` for complex scenarios

#### Examples:
```csharp
// "It"-Pattern (preferred for base tests)
[Fact]
public void ItHasTraineeId() { }

[Fact]
public void ItHasDomain() { }

[Fact]
public void ItHasLetter() { }

// From BE.League.Desktop Models:
[Fact]
public void ItCanBeInstantiated() { }

[Fact]
public void ItHasEmptyEventsListByDefault() { }

[Fact]
public void ItCanSetSummonerId() { }

// "Action_Condition_Result"-Pattern
[Fact]
public async Task CreateWks_WithValidCommand_CreatesWorksheetWithCorrectProperties() { }

[Fact]
public async Task CreateWks_WithEmptyLearnPlan_CreatesWorksheetWithDefaultTasks() { }

// From BE.League.Desktop:
[Fact]
public async Task GetAllGameDataAsync_WithValidJson_ReturnsDeserializedObject() { }

[Fact]
public async Task GetActivePlayerAsync_WithNullJson_ReturnsNull() { }

[Fact]
public async Task GetPlayerListAsync_WithEmptyArray_ReturnsEmptyList() { }

// Negation tests
[Fact]
public void ItHasNoRelatedLetterOnNormalChar(char val) { }

// Behavior tests
[Fact]
public void WasTooSlow_ReturnsTrueForSlowTasks() { }

[Fact]
public void NonFlawlessNonEmptyTasks_FiltersCorrectly() { }
```

### 2.3 Test Folder Structure

```
BE.Learning.Tests/
├── {DomainConcept}Tests/           # e.g., LetterRecognitionTests
│   ├── Given{MainClass}.cs
│   ├── {Helper}.cs
│   └── Nested folders for subconcepts
├── {Feature}Tests/                 # e.g., LearnplanStateFactTests
│   ├── Given{Feature}.cs           # Base test class
│   ├── When{Scenario}.cs           # Scenario tests
│   ├── Events/                     # Test events
│   │   └── Test{EventName}.cs
│   └── {SubFeature}/               # Nested scenarios
│       └── When{DetailedScenario}.cs
└── Helper classes (directly in root)
    ├── FixedTimeProvider.cs
    └── SampleWorksheetTask.cs

BE.League.Desktop.Tests/
├── LiveClientObjectReaderTests/    # Tests for reader/deserializer
│   ├── GivenLiveClientObjectReader.cs
│   ├── WhenDeserializingAllGameData.cs
│   ├── WhenDeserializingActivePlayer.cs
│   ├── WhenDeserializingPlayerList.cs
│   ├── WhenDeserializingChampSelectSession.cs
│   └── ... (organized by API method/data type)
└── ModelTests/                     # Tests for data models
    ├── GivenChampSelectSession.cs
    ├── GivenLiveEvent.cs
    ├── GivenLobbyMember.cs
    └── ... (one file per model)
```

**Examples from BE.Learning:**
```
LearnplanStateFactTests/
├── GivenLearnplanStateFact.cs
├── TestState.cs
├── WhenCreated.cs
├── WhenLearnPlanCreated.cs
├── Events/
│   ├── TestLearnPlanCreated.cs
│   ├── TestLearnPlanResetted.cs
│   └── ...
└── AnsweringBehavior/
    ├── WhenCreatedAndTraineeAnsweredOnCurrent.cs
    ├── WhenCreatedAndTraineeAnsweredOnRepetition.cs
    └── ...
```

---

## 3. Test Structures & Patterns

### 3.1 Base Test Class Pattern

**Abstract Given-Class** as base for scenario-based tests:

```csharp
public abstract class GivenLearnplanStateFact
{
    protected TestState Sut { get; }

    public GivenLearnplanStateFact()
    {
        Sut = GetSut();
        
        var events = new List<IEvent>();
        EventsForSut(events);
        
        Sut.Execute(events);
    }

    // Common assert helpers
    public void AssertSingleItem(ISet<string> items, string item) =>
        Assert.Single(items, x => x == item);

    // Override points for derived classes
    protected virtual void EventsForSut(List<IEvent> events) { }
    
    protected virtual TestState GetSut()
    {
        return new();
    }

    protected void AssertNotNullButEmpty<T>(ISet<T> items)
    {
        Assert.NotNull(items);
        Assert.Empty(items);
    }
}
```

**Example from BE.League.Desktop** - Base class with mocked dependencies:

```csharp
public class GivenLiveClientObjectReader
{
    protected readonly ILeagueDesktopClient Gateway;
    protected readonly LiveClientObjectReader Sut;

    public GivenLiveClientObjectReader()
    {
        Gateway = A.Fake<ILeagueDesktopClient>();
        Sut = new LiveClientObjectReader(Gateway);
    }

    [Fact]
    public void ItCanBeConstructedWithGateway()
    {
        var gateway = A.Fake<ILeagueDesktopClient>();
        var sut = new LiveClientObjectReader(gateway);
        
        Assert.NotNull(sut);
    }
}
```

**Derived Scenario Tests:**

```csharp
public sealed class WhenCreated : GivenLearnplanStateFact
{
    [Fact]
    public void ItHasNonNullCurrent() =>
        AssertNotNullButEmpty(Sut.Current);

    [Fact]
    public void ItHasUpNextNonNull() =>
        AssertNotNullButEmpty(Sut.UpNext);
}

// BE.League.Desktop example:
public sealed class WhenDeserializingActivePlayer : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetActivePlayerAsync_WithValidJson_ReturnsDeserializedObject()
    {
        var json = """
        {
            "level": 10,
            "summonerName": "PlayerOne"
        }
        """;

        A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        var result = await Sut.GetActivePlayerAsync();

        Assert.NotNull(result);
        Assert.Equal(10, result.Level);
    }
}
```

### 3.2 Simple Given-Class Pattern

For tests without complex setup:

```csharp
public class GivenWorksheet
{
    // Test data as properties
    public TraineeId TraineeId { get; init; } = new("tre_1231231");
    public WorksheetDomain Domain { get; init; } = WorksheetDomainList.RecognizeGermanLetters;
    public DateTimeOffset Now { get; init; } = new DateTimeOffset(2025, 2, 14, 13, 12, 1, TimeSpan.FromHours(1));

    // Factory method for SUT (System Under Test)
    protected virtual async Task<Worksheet<SampleWorksheetTask>> GetSut()
    {
        var prov = new FixedTimeProvider(Now);
        var id = await WorksheetId.New();
        return await Worksheet<SampleWorksheetTask>.Blank(id, TraineeId, Domain, prov);
    }

    [Fact]
    public async Task ItHasTraineeId()
    {
        var sut = await GetSut();
        Assert.Equal(TraineeId, sut.TraineeId);
    }
}
```

**BE.League.Desktop Model Test Pattern:**

```csharp
public sealed class GivenLobbyMember
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new LobbyMember();
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItCanSetSummonerId()
    {
        var sut = new LobbyMember { SummonerId = 12345L };
        Assert.Equal(12345L, sut.SummonerId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsLeader(bool isLeader)
    {
        var sut = new LobbyMember { IsLeader = isLeader };
        Assert.Equal(isLeader, sut.IsLeader);
    }
}
```

### 3.3 Test Event Classes

Test events implement the same interface as real events:

```csharp
public sealed class TestLearnPlanCreated : EventBase, ILearnPlanCreatedEvent
{
    // Static test constants
    public const string UpNextValue = "UpNext";
    public const string CurrentValue = "Current";
    public const string StartedValue = "Started";
    public const string RepetitionsValue = "Repetitions";

    // Static test subjects
    public static readonly LearnSubject UpNextSubject = new(UpNextValue, UpNextValue);
    public static readonly LearnSubject CurrentSubject = new(CurrentValue, CurrentValue);
    public static readonly LearnSubject StartedSubject = new(StartedValue, StartedValue);
    public static readonly LearnSubject RepetitionsSubject = new(RepetitionsValue, RepetitionsValue);

    // Event properties
    public HashSet<LearnSubjectEventFacet>? Current { get; } = [CurrentSubject];
    public HashSet<LearnSubjectEventFacet>? UpNext { get; } = [UpNextSubject];
    public HashSet<LearnSubjectEventFacet>? Started { get; } = [StartedSubject];
    public HashSet<LearnSubjectEventFacet>? Repetitions { get; } = [RepetitionsSubject];
}
```

### 3.4 Test Helper Classes

#### FixedTimeProvider
```csharp
public sealed class FixedTimeProvider(DateTimeOffset fixedTime) : TimeProvider
{
    public DateTimeOffset Now => fixedTime;

    public override DateTimeOffset GetUtcNow()
    {
        return Now.UtcDateTime;
    }
}
```

#### Sample Domain Objects
```csharp
public sealed class SampleWorksheetTask : WorksheetTaskBase<string>
{
    public SampleWorksheetTask(string key, string displayText, bool isSeperatorState, int priority) 
        : base(TaskId.New(), key, key, displayText, isSeperatorState)
    {
    }
}
```

---

## 4. xUnit Test Attributes & Patterns

### 4.1 [Fact]
For tests without parameters:

```csharp
[Fact]
public void ItHasStaticCultures()
{
    Assert.Equal("deu", TaskCulture.Deu.Key);
    Assert.Equal("de-DE", TaskCulture.Deu.Name);
}

// BE.League.Desktop example:
[Fact]
public void ItCanBeInstantiated()
{
    var sut = new LiveEvent();
    Assert.NotNull(sut);
}
```

### 4.2 [Theory] with [InlineData]
For parameterized tests:

```csharp
[Theory]
[InlineData("de-DE", "deu", "de-DE")]
[InlineData("en-US", "eng", "en-US")]
[InlineData("fr-FR", "fra", "fr-FR")]
public void ItConvertsFromCulture(string cultureName, string expectedKey, string expectedName)
{
    var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
    var result = TaskCulture.From(cultureInfo);

    Assert.Equal(expectedKey, result.Key);
    Assert.Equal(expectedName, result.Name);
}

// BE.League.Desktop example:
[Theory]
[InlineData(true)]
[InlineData(false)]
public void ItCanSetIsLeader(bool isLeader)
{
    var sut = new LobbyMember { IsLeader = isLeader };
    Assert.Equal(isLeader, sut.IsLeader);
}
```

### 4.3 Multiple [InlineData] for Different Scenarios

```csharp
[Theory]
[InlineData("Test", false, true)]   // RealValue
[InlineData("", false, false)]      // NoneSeperator - Empty
[InlineData(" ", false, false)]     // NonSeperator - WhiteSpaceOnly
[InlineData("Test", true, false)]   // Separator - WithValue
[InlineData("", true, false)]       // Separator - EmptyValue
public void IsNonSeperatorNonEmptyTask_ReturnsExpectedResult(string value, bool isSeperator, bool expect)
{
    var task = new TestWorksheetTaskWithStringValue(value, isSeperator);
    var result = task.IsNonSeperatorNonEmptyTask();
    Assert.Equal(expect, result);
}

// BE.League.Desktop example - testing different states:
[Theory]
[InlineData("Pending")]
[InlineData("Accepted")]
[InlineData("Declined")]
public void ItCanSetState(string state)
{
    var sut = new LobbyInvitation { State = state };
    Assert.Equal(state, sut.State);
}
```

---

## 5. Assert Patterns

### 5.1 Basic Assertions
```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Null/Not-Null
Assert.Null(value);
Assert.NotNull(value);

// Boolean
Assert.True(condition);
Assert.False(condition);

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Single(collection);
Assert.Contains(item, collection);
```

### 5.2 Collection Assertions with Lambda
```csharp
// Single with predicate
Assert.Single(items, x => x == item);
Assert.Single(items, x => x.Equals(item));

// Contains with predicate
Assert.Contains(tasks, task => task.Letter.Value == 'C');

// BE.League.Desktop example:
Assert.Single(result.Members, m => m.SummonerId == 123);
```

### 5.3 Assert.Collection for Ordered Assertions
```csharp
Assert.Collection(result,
    item => Assert.Equal("Task1", item.Subject.Value),
    item => Assert.Equal("Task3", item.Subject.Value)
);

// BE.League.Desktop example:
Assert.Collection(result.Events,
    evt => Assert.Equal("GameStart", evt.EventName),
    evt => Assert.Equal("FirstBlood", evt.EventName)
);
```

### 5.4 Type Assertions
```csharp
var firstTask = Assert.IsType<LetterRecognitionTask>(created.Worksheet.Planned.First());

// BE.League.Desktop example:
var activePlayer = Assert.IsType<ActivePlayer>(result);
```

### 5.5 Exception Assertions
```csharp
Assert.Throws<ArgumentNullException>(() => TaskCulture.Parse(cultureName));
Assert.Throws<ArgumentException>(() => TaskCulture.Parse(cultureName));
Assert.Throws<CultureNotFoundException>(() => TaskCulture.Parse("invalid-culture"));

// Async exceptions
await Assert.ThrowsAsync<InvalidOperationException>(async () => 
    await sut.PerformOperationAsync());
```

---

## 6. Test-Daten-Management

### 6.1 Konstanten in Test-Klassen
```csharp
public class GivenWorksheet
{
    public TraineeId TraineeId { get; init; } = new("tre_1231231");
    public WorksheetDomain Domain { get; init; } = WorksheetDomainList.RecognizeGermanLetters;
    public DateTimeOffset Now { get; init; } = new DateTimeOffset(2025, 2, 14, 13, 12, 1, TimeSpan.FromHours(1));
}
```

---

## 6. Test Data Management

### 6.1 Constants in Test Classes
```csharp
public class GivenWorksheet
{
    public TraineeId TraineeId { get; init; } = new("tre_1231231");
    public WorksheetDomain Domain { get; init; } = WorksheetDomainList.RecognizeGermanLetters;
    public DateTimeOffset Now { get; init; } = new DateTimeOffset(2025, 2, 14, 13, 12, 1, TimeSpan.FromHours(1));
}
```

### 6.2 Factory Methods
```csharp
private static CompletedTask CreateCompletedTask(
    string value,
    bool isSeperator,
    TimeSpan elapsed,
    CompletedState state)
{
    var task = new TestWorksheetTaskWithStringValue(value, isSeperator);
    return new CompletedTask(task, state, elapsed, DateTimeOffset.Now);
}
```

### 6.3 Static Test Data in Test Events
```csharp
public sealed class TestLearnPlanCreated : EventBase, ILearnPlanCreatedEvent
{
    public const string UpNextValue = "UpNext";
    public static readonly LearnSubject UpNextSubject = new(UpNextValue, UpNextValue);
}
```

---

## 7. Mocking with FakeItEasy

### 7.1 Basic Fake Creation
```csharp
private readonly IWorksheetGateway _gateway;
private readonly ICommandBus _commandBus;

public GivenLetterRecognitionWorksheetDomainObject()
{
    _gateway = A.Fake<IWorksheetGateway>();
    _commandBus = A.Fake<ICommandBus>();
    var mapper = A.Fake<ITraineeDomainobjectMapper>();
}

// BE.League.Desktop example:
public class GivenLiveClientObjectReader
{
    protected readonly ILeagueDesktopClient Gateway;
    
    public GivenLiveClientObjectReader()
    {
        Gateway = A.Fake<ILeagueDesktopClient>();
    }
}
```

### 7.2 Configuring Fakes with A.CallTo
```csharp
// Simple return value
A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
    .Returns(Task.FromResult<string?>(json));

// Conditional returns
A.CallTo(() => _gateway.GetByIdAsync(A<WorksheetId>._, A<CancellationToken>._))
    .Returns(Task.FromResult<Worksheet<SampleTask>?>(existingWorksheet));

// Multiple return values in sequence
A.CallTo(() => _service.GetValue())
    .ReturnsNextFromSequence("First", "Second", "Third");
```

### 7.3 Verifying Calls
```csharp
// Verify a call happened
A.CallTo(() => Gateway.AcceptReadyCheckAsync(A<CancellationToken>._))
    .MustHaveHappenedOnceExactly();

// Verify a call never happened
A.CallTo(() => Gateway.DeclineReadyCheckAsync(A<CancellationToken>._))
    .MustNotHaveHappened();

// Verify with specific parameters
A.CallTo(() => Gateway.GetPlayerScoresJsonAsync("TestPlayer", A<CancellationToken>._))
    .MustHaveHappenedOnceExactly();
```

### 7.4 Argument Matching
```csharp
// Any value of type
A.CallTo(() => method(A<string>._))

// Specific value
A.CallTo(() => method("specific"))

// Predicate matching
A.CallTo(() => method(A<string>.That.StartsWith("Test")))

// Ignored arguments
A.CallTo(() => method(A<CancellationToken>.Ignored))
```

### 7.5 In-Memory Implementations Instead of Mocks
Where possible, use real in-memory implementations:

```csharp
_learnPlanGateway = new InMemoryLetterRecognitionLearnPlanGateway();

// Advantages:
// - Tests real behavior, not mocked behavior
// - Less brittle tests
// - Easier to maintain
// - Better integration test coverage
```

---

## 8. Async/Await in Tests

### 8.1 Async Test Methods
```csharp
[Fact]
public async Task ItHasTraineeId()
{
    var sut = await GetSut();
    Assert.Equal(TraineeId, sut.TraineeId);
}

// BE.League.Desktop example:
[Fact]
public async Task GetAllGameDataAsync_WithValidJson_ReturnsDeserializedObject()
{
    var json = """{"gameData": {"gameMode": "CLASSIC"}}""";
    A.CallTo(() => Gateway.GetAllGameDataJsonAsync(A<CancellationToken>._))
        .Returns(Task.FromResult<string?>(json));

    var result = await Sut.GetAllGameDataAsync();

    Assert.NotNull(result);
}
```

### 8.2 Async Factory Methods
```csharp
protected virtual async Task<Worksheet<SampleWorksheetTask>> GetSut()
{
    var prov = new FixedTimeProvider(Now);
    var id = await WorksheetId.New();
    return await Worksheet<SampleWorksheetTask>.Blank(id, TraineeId, Domain, prov);
}
```

### 8.3 Testing Async Exceptions
```csharp
[Fact]
public async Task MethodAsync_WithInvalidInput_ThrowsException()
{
    await Assert.ThrowsAsync<InvalidOperationException>(
        async () => await Sut.MethodAsync(null));
}
```

---

## 9. Test Organization Best Practices

### 9.1 Arrange-Act-Assert Pattern
```csharp
[Fact]
public async Task CreateWks_WithValidCommand_CreatesWorksheetWithCorrectProperties()
{
    // Arrange
    var customPlan = new LetterRecognitionLearnPlan(...);
    await _learnPlanGateway.CreateForTrainee(_traineeId, customPlan);
    var command = new CreateRecognizeLetterWksForUserCommmand(...);

    // Act
    await _sut.On(command);

    // Assert
    var events = _sut.GetUncommittedEvents();
    Assert.Single(events);
}

// BE.League.Desktop example:
[Fact]
public async Task GetActivePlayerAsync_WithValidJson_ReturnsDeserializedObject()
{
    // Arrange
    var json = """{"level": 10, "summonerName": "TestPlayer"}""";
    A.CallTo(() => Gateway.GetActivePlayerJsonAsync(A<CancellationToken>._))
        .Returns(Task.FromResult<string?>(json));

    // Act
    var result = await Sut.GetActivePlayerAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(10, result.Level);
    Assert.Equal("TestPlayer", result.SummonerName);
}
```

### 9.2 One Assert per Test (where possible)
```csharp
[Fact]
public void ItHasNonNullCurrent() =>
    AssertNotNullButEmpty(Sut.Current);

[Fact]
public void ItHasUpNextNonNull() =>
    AssertNotNullButEmpty(Sut.UpNext);

// Exception: When testing an object's state after creation
[Fact]
public void ItCanBeInstantiated()
{
    var sut = new LobbyMember();
    Assert.NotNull(sut);  // Multiple asserts OK when testing creation
    Assert.Empty(sut.Members);
}
```

### 9.3 Descriptive Test Names over Comments
```csharp
// Good: Name explains what is being tested
[Theory]
[InlineData("Test", false, true)]   // RealValue
[InlineData("", false, false)]      // NonSeperator - Empty
[InlineData(" ", false, false)]     // NonSeperator - WhiteSpaceOnly
public void IsNonSeperatorNonEmptyTask_ReturnsExpectedResult(...)

// Better: Method name + InlineData comments provide full context
```

### 9.4 Grouping Related Tests
```csharp
// Use nested classes for related scenarios
public class GivenLobbyMember
{
    [Fact]
    public void ItCanBeInstantiated() { }
    
    // Group boolean property tests
    public class BooleanProperties : GivenLobbyMember
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ItCanSetIsLeader(bool value) { }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ItCanSetReady(bool value) { }
    }
}
```

### 9.5 Test Independence
```csharp
// BAD: Tests depend on each other
private static int _counter = 0;

[Fact]
public void Test1() { _counter++; }

[Fact]
public void Test2() { Assert.Equal(1, _counter); } // Fails if Test1 doesn't run first

// GOOD: Each test is independent
[Fact]
public void Test1() 
{ 
    var counter = 0;
    counter++;
    Assert.Equal(1, counter);
}

[Fact]
public void Test2() 
{ 
    var counter = 1;
    Assert.Equal(1, counter);
}
}
```

### 9.2 One Assert per Test (wo möglich)
```csharp
[Fact]
public void ItHasNonNullCurrent() =>
    AssertNotNullButEmpty(Sut.Current);

[Fact]
public void ItHasUpNextNonNull() =>
    AssertNotNullButEmpty(Sut.UpNext);
```

### 9.3 Descriptive Test Names über Code
```csharp
// Kommentare in InlineData für Klarheit
[Theory]
[InlineData("Test", false, true)]   // RealValue
[InlineData("", false, false)]      // NoneSeperator - Empty
[InlineData(" ", false, false)]     // NonSeperator - WhiteSpaceOnly
```

---

## 10. Checkliste für neue Test-Klassen

### Für eine neue Domain (z.B. TextRecognition):

- [ ] **Ordner erstellen**: `BE.Learning.Tests/TextRecognitionTests/`
- [ ] **Basis-Test-Klasse**: `GivenTextRecognitionWorksheetDomainObject.cs`
- [ ] **Test-Helper**: Sample-Tasks, Test-Events falls benötigt
- [ ] **Namenskonvention**: `Given{Class}` für Basis, `When{Scenario}` für Szenarien
- [ ] **Test-Methoden**: `It{ExpectedBehavior}` oder `{Action}_{Condition}_{Result}`
- [ ] **xUnit-Attribute**: `[Fact]` für einfache Tests, `[Theory]` mit `[InlineData]` für parametrisierte
- [ ] **Mocking**: FakeItEasy für Interfaces, In-Memory für Gateways wenn möglich
- [ ] **Async**: Async/await für asynchrone Operationen
- [ ] **Assertions**: Single, Equal, NotNull, etc. aus xUnit
- [ ] **Test-Daten**: Konstanten, Factory-Methoden, statische Test-Objekte
- [ ] **Ordnerstruktur**: Nested Folders für Sub-Features

---

## 11. Beispiel-Template für neue Domain

```csharp
using BE.Learning.TextRecognition; // Anpassen
using BE.Learning.SharedKernel.Trainees;
using BE.Learning.Worksheets;

namespace BE.Learning.Tests.TextRecognitionTests;

public class GivenTextRecognitionWorksheetDomainObject
{
    private readonly ITextRecognitionGateway _gateway;
    private readonly TextRecognitionWksCreationStrategy _creationStrategy;
    private readonly TextRecognitionWorksheetDomainObject _sut;
    private readonly TraineeId _traineeId;

    public GivenTextRecognitionWorksheetDomainObject()
    {
        // Arrange - Setup
        _gateway = new InMemoryTextRecognitionGateway();
        _creationStrategy = new TextRecognitionWksCreationStrategy(_gateway);
        _traineeId = TraineeId.New().Result;
        
        _sut = new TextRecognitionWorksheetDomainObject(
            "test-id", 
            _creationStrategy, 
            _gateway);
    }

    [Fact]
    public async Task CreateWks_WithValidCommand_CreatesWorksheetWithCorrectProperties()
    {
        // Arrange
        var command = new CreateTextRecognitionWksCommand(_traineeId);

        // Act
        await _sut.On(command);

        // Assert
        var events = _sut.GetUncommittedEvents();
        Assert.Single(events);
    }

}
```

---

## 10. Checklist for New Test Classes

### For a new Domain (e.g., TextRecognition):

- [ ] **Create folder**: `BE.Learning.Tests/TextRecognitionTests/`
- [ ] **Base test class**: `GivenTextRecognitionWorksheetDomainObject.cs`
- [ ] **Test helpers**: Sample tasks, test events if needed
- [ ] **Naming convention**: `Given{Class}` for base, `When{Scenario}` for scenarios
- [ ] **Test methods**: `It{ExpectedBehavior}` or `{Action}_{Condition}_{Result}`
- [ ] **xUnit attributes**: `[Fact]` for simple tests, `[Theory]` with `[InlineData]` for parameterized
- [ ] **Mocking**: FakeItEasy for interfaces, in-memory for gateways when possible
- [ ] **Async**: Use async/await for asynchronous operations
- [ ] **Assertions**: Single, Equal, NotNull, etc. from xUnit
- [ ] **Test data**: Constants, factory methods, static test objects
- [ ] **Folder structure**: Nested folders for sub-features

### For API/Reader Classes (like LiveClientObjectReader):

- [ ] **Create folder**: `{ComponentName}Tests/` (e.g., `LiveClientObjectReaderTests/`)
- [ ] **Base test class**: `Given{ComponentName}.cs` with mocked dependencies
- [ ] **Scenario tests**: `WhenDeserializing{DataType}.cs`, `WhenCalling{Method}.cs`
- [ ] **Test JSON data**: Use raw string literals (`"""..."""`) for test JSON
- [ ] **Null handling**: Test null, empty, whitespace inputs
- [ ] **Error cases**: Test invalid JSON, malformed data
- [ ] **Token passing**: Verify CancellationToken is passed correctly
- [ ] **Mock verification**: Use `MustHaveHappenedOnceExactly()` to verify calls

### For Model/DTO Classes:

- [ ] **One file per model**: `Given{ModelName}.cs`
- [ ] **Instantiation test**: Verify model can be created
- [ ] **Property tests**: Test each settable property
- [ ] **Default values**: Test collections are initialized (not null)
- [ ] **Theory tests**: Use `[Theory]` for boolean/enum properties
- [ ] **Keep it simple**: Models usually don't need complex setup

---

## 11. Example Template for New Domain

```csharp
using BE.Learning.TextRecognition; // Adjust
using BE.Learning.SharedKernel.Trainees;
using BE.Learning.Worksheets;

namespace BE.Learning.Tests.TextRecognitionTests;

public class GivenTextRecognitionWorksheetDomainObject
{
    private readonly ITextRecognitionGateway _gateway;
    private readonly TextRecognitionWksCreationStrategy _creationStrategy;
    private readonly TextRecognitionWorksheetDomainObject _sut;
    private readonly TraineeId _traineeId;

    public GivenTextRecognitionWorksheetDomainObject()
    {
        // Arrange - Setup
        _gateway = new InMemoryTextRecognitionGateway();
        _creationStrategy = new TextRecognitionWksCreationStrategy(_gateway);
        _traineeId = TraineeId.New().Result;
        
        _sut = new TextRecognitionWorksheetDomainObject(
            "test-id", 
            _creationStrategy, 
            _gateway);
    }

    [Fact]
    public async Task CreateWks_WithValidCommand_CreatesWorksheetWithCorrectProperties()
    {
        // Arrange
        var command = new CreateTextRecognitionWksCommand(_traineeId);

        // Act
        await _sut.On(command);

        // Assert
        var events = _sut.GetUncommittedEvents();
        Assert.Single(events);
    }

    [Theory]
    [InlineData("Text1", true)]
    [InlineData("Text2", false)]
    public void CreateWks_WithDifferentTexts_ReturnsExpectedResult(string text, bool expected)
    {
        // Arrange & Act & Assert
    }
}
```

---

## 12. Example Template for API Reader/Deserializer

```csharp
using FakeItEasy;

namespace BE.League.Desktop.Tests.LiveClientObjectReaderTests;

public class GivenLiveClientObjectReader
{
    protected readonly ILeagueDesktopClient Gateway;
    protected readonly LiveClientObjectReader Sut;

    public GivenLiveClientObjectReader()
    {
        Gateway = A.Fake<ILeagueDesktopClient>();
        Sut = new LiveClientObjectReader(Gateway);
    }

    [Fact]
    public void ItCanBeConstructedWithGateway()
    {
        var gateway = A.Fake<ILeagueDesktopClient>();
        var sut = new LiveClientObjectReader(gateway);
        
        Assert.NotNull(sut);
    }
}

// Scenario-based test
public sealed class WhenDeserializingSomeData : GivenLiveClientObjectReader
{
    [Fact]
    public async Task GetDataAsync_WithValidJson_ReturnsDeserializedObject()
    {
        // Arrange
        var json = """
        {
            "property": "value"
        }
        """;
        
        A.CallTo(() => Gateway.GetDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(json));

        // Act
        var result = await Sut.GetDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("value", result.Property);
    }

    [Fact]
    public async Task GetDataAsync_WithNullJson_ReturnsNull()
    {
        // Arrange
        A.CallTo(() => Gateway.GetDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>(null));

        // Act
        var result = await Sut.GetDataAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDataAsync_WithInvalidJson_ReturnsNull()
    {
        // Arrange
        A.CallTo(() => Gateway.GetDataJsonAsync(A<CancellationToken>._))
            .Returns(Task.FromResult<string?>("{invalid}"));

        // Act
        var result = await Sut.GetDataAsync();

        // Assert
        Assert.Null(result);
    }
}
```

---

## 13. Example Template for Model Tests

```csharp
namespace BE.League.Desktop.Tests.ModelTests;

public sealed class GivenMyModel
{
    [Fact]
    public void ItCanBeInstantiated()
    {
        var sut = new MyModel();
        
        Assert.NotNull(sut);
    }

    [Fact]
    public void ItHasEmptyCollectionByDefault()
    {
        var sut = new MyModel();
        
        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
    }

    [Fact]
    public void ItCanSetId()
    {
        var sut = new MyModel { Id = 123 };
        
        Assert.Equal(123, sut.Id);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ItCanSetIsActive(bool isActive)
    {
        var sut = new MyModel { IsActive = isActive };
        
        Assert.Equal(isActive, sut.IsActive);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    [InlineData("Value3")]
    public void ItCanSetName(string name)
    {
        var sut = new MyModel { Name = name };
        
        Assert.Equal(name, sut.Name);
    }
}
```

---

## 14. Important Guidelines

### DO's:
✅ Use descriptive test names with `Given`/`When`/`It` prefixes  
✅ Use `[Theory]` for parameterized tests  
✅ Create helper methods for common assertions  
✅ Use in-memory implementations where possible instead of mocks  
✅ Keep tests focused and small (one assert where sensible)  
✅ Organize tests in logical folders by feature  
✅ Use `async/await` for asynchronous operations  
✅ Create test events that implement real interfaces  
✅ Avoid duplicated code - generalize and create common methods  
✅ Test edge cases: null, empty, whitespace, invalid input  
✅ Verify mock interactions when testing integration points  
✅ Use raw string literals (`"""..."""`) for multi-line JSON  

### DON'Ts:
❌ No generic test names like `Test1`, `TestMethod`  
❌ No large monolithic test classes  
❌ No test logic in production code  
❌ No shared mutable state between tests  
❌ No excessive mocks when real objects are available  
❌ No comments in tests unless necessary for non-obvious semantics  
❌ No large test methods - break them down  
❌ No hard-coded magic numbers without context  
❌ No testing implementation details - test behavior  
❌ No forgetting to test null/error cases  

---

## 15. Testing Best Practices from BE.League.Desktop

### 15.1 Deserialization Testing Pattern
When testing JSON deserializers, always test:

1. **Happy path** - valid JSON returns correct object
2. **Null input** - returns null gracefully
3. **Empty string** - returns null gracefully
4. **Whitespace only** - returns null gracefully
5. **Invalid JSON** - returns null (doesn't throw)
6. **CancellationToken** - is passed to underlying methods

Example:
```csharp
[Fact]
public async Task GetDataAsync_WithValidJson_ReturnsDeserializedObject() { }

[Fact]
public async Task GetDataAsync_WithNullJson_ReturnsNull() { }

[Fact]
public async Task GetDataAsync_WithEmptyString_ReturnsNull() { }

[Fact]
public async Task GetDataAsync_WithWhitespaceString_ReturnsNull() { }

[Fact]
public async Task GetDataAsync_WithInvalidJson_ReturnsNull() { }

[Fact]
public async Task GetDataAsync_WithCancellationToken_PassesTokenToGateway() { }
```

### 15.2 Model Testing Pattern
For simple DTOs/Models, test:

1. **Instantiation** - can be created
2. **Default values** - collections are initialized, not null
3. **Property setters** - each property can be set
4. **Multiple values** - use `[Theory]` for enums/booleans

Example:
```csharp
[Fact]
public void ItCanBeInstantiated() { }

[Fact]
public void ItHasEmptyMembersArrayByDefault() { }

[Fact]
public void ItCanSetPropertyX() { }

[Theory]
[InlineData(true)]
[InlineData(false)]
public void ItCanSetBooleanProperty(bool value) { }
```

### 15.3 Base Class Inheritance Pattern
Use base classes to share setup and provide common tests:

```csharp
// Base class with common setup
public class GivenLiveClientObjectReader
{
    protected readonly ILeagueDesktopClient Gateway;
    protected readonly LiveClientObjectReader Sut;

    public GivenLiveClientObjectReader()
    {
        Gateway = A.Fake<ILeagueDesktopClient>();
        Sut = new LiveClientObjectReader(Gateway);
    }

    // Common tests that all derived classes inherit
    [Fact]
    public void ItCanBeConstructedWithGateway() { }
}

// Derived scenario class
public sealed class WhenDeserializingX : GivenLiveClientObjectReader
{
    // Specific tests for this scenario
    [Fact]
    public async Task SpecificTest() { }
}
```

### 15.4 JSON Test Data with Raw String Literals
Use C# 11 raw string literals for readable JSON:

```csharp
var json = """
{
    "level": 10,
    "summonerName": "TestPlayer",
    "nested": {
        "property": "value"
    }
}
""";
```

### 15.5 FakeItEasy Verification Pattern
Always verify important interactions:

```csharp
// Setup
A.CallTo(() => Gateway.MethodAsync(A<CancellationToken>._))
    .Returns(Task.FromResult<string?>(json));

// Act
await Sut.MethodAsync();

// Verify
A.CallTo(() => Gateway.MethodAsync(A<CancellationToken>._))
    .MustHaveHappenedOnceExactly();
```

---

## 16. Resources & Links

- **xUnit Documentation**: https://xunit.net/
- **FakeItEasy Documentation**: https://fakeiteasy.github.io/
- **Coverlet**: https://github.com/coverlet-coverage/coverlet
- **C# Raw String Literals**: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string

---

**Version**: 2.0  
**Last Updated**: 2025-10-31  
**Projects**: BE.Learning, BE.League.Desktop

