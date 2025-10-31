# Test Structure Documentation

## Overview

This documentation describes the test structure, naming conventions, and tools used in our projects. It serves as a reference for creating consistent unit tests across different domains and projects.

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


public class GivenLiveClientObjectReader { }
public class GivenChampSelectSession { }
public class GivenLobbyMember { }

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

[Fact]
public void ItCanBeInstantiated() { }

[Fact]
public void ItHasEmptyEventsListByDefault() { }

[Fact]
public void ItCanSetSummonerId() { }


// From BE.League.Desktop:
[Fact]
public async Task GetAllGameDataAsync_WithValidJson_ReturnsDeserializedObject() { }

[Fact]
public async Task GetActivePlayerAsync_WithNullJson_ReturnsNull() { }

[Fact]
public async Task GetPlayerListAsync_WithEmptyArray_ReturnsEmptyList() { }


// Behavior tests
[Fact]
public void WasTooSlow_ReturnsTrueForSlowTasks() { }

[Fact]
public void NonFlawlessNonEmptyTasks_FiltersCorrectly() { }
```

### 2.3 Test Folder Structure

```
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

---


## 4. Mocking with FakeItEasy

### 4.2 Gateway Mocking
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

### 4.2 Verifying Calls
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



---

## 5. Test Organization Best Practices

### 5.1 Arrange-Act-Assert Pattern
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

### 5.2 One Assert per Test (where possible)
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

### 5.3 Descriptive Test Names over Comments
```csharp
// Good: Name explains what is being tested
[Theory]
[InlineData("Test", false, true)]   // RealValue
[InlineData("", false, false)]      // NonSeperator - Empty
[InlineData(" ", false, false)]     // NonSeperator - WhiteSpaceOnly
public void IsNonSeperatorNonEmptyTask_ReturnsExpectedResult(...)

// Better: Method name + InlineData comments provide full context
```

### 5.4 Grouping Related Tests
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

### 5.5 Test Independence
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

### 5.6 One Assert per Test (wo möglich)
```csharp
[Fact]
public void ItHasNonNullCurrent() =>
    AssertNotNullButEmpty(Sut.Current);

[Fact]
public void ItHasUpNextNonNull() =>
    AssertNotNullButEmpty(Sut.UpNext);
```

### 5.7 Descriptive Test Names über Code
```csharp
// Kommentare in InlineData für Klarheit
[Theory]
[InlineData("Test", false, true)]   // RealValue
[InlineData("", false, false)]      // NoneSeperator - Empty
[InlineData(" ", false, false)]     // NonSeperator - WhiteSpaceOnly
```

---

## 6. Checkliste für neue Test-Klassen

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

## 8. Checklist for New Test Classes

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


## 9. Example Template for Model Tests

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

## 10. Important Guidelines

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

## 11. Testing Best Practices from BE.League.Desktop

### 11.1 Deserialization Testing Pattern
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
---
## 16. Resources & Links

- **xUnit Documentation**: https://xunit.net/
- **FakeItEasy Documentation**: https://fakeiteasy.github.io/
- **Coverlet**: https://github.com/coverlet-coverage/coverlet
- **C# Raw String Literals**: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/raw-string

---

**Version**: 1.0  
**Last Updated**: 2025-10-31  
**Projects**:  BE.League.Desktop

