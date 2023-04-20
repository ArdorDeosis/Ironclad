using NUnit.Framework;

namespace Ironclad.Collections.Tests;

[TestFixture]
public class ValueListTests
{
  [Test]
  public void Empty_ShouldBeEmpty()
  {
    // ARRANGE
    var empty = ValueList<int>.Empty;

    // ASSERT
    Assert.That(empty, Is.Empty);
  }

  [Test]
  public void Empty_ShouldEqual_ValueListFromEmptyInput()
  {
    // ARRANGE
    var empty = ValueList<int>.Empty;
    var fromDefaultConstructor = new ValueList<int>();
    var fromEmptyCollection = new ValueList<int>(Array.Empty<int>());

    // ASSERT
    Assert.That(empty, Is.EqualTo(fromDefaultConstructor));
    Assert.That(empty, Is.EqualTo(fromEmptyCollection));
  }

  [TestCaseSource(nameof(ValueListsAndCounts))]
  public void Count_ShouldReturnNumberOfItems(ValueList<object> list, int expectedCount)
  {
    // ASSERT
    Assert.That(list, Has.Count.EqualTo(expectedCount));
  }

  [Test]
  public void Indexer_ValidIndex_ShouldReturnExpectedValue()
  {
    // ARRANGE
    var list = new ValueList<int>(1, 2, 3);

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(list[0], Is.EqualTo(1));
      Assert.That(list[1], Is.EqualTo(2));
      Assert.That(list[2], Is.EqualTo(3));
    });
  }

  [Test]
  public void Indexer_IndexOutOfRange_ShouldThrow()
  {
    // ARRANGE
    var list = new ValueList<int>(0xC0FFEE);

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(() => list[-1], Throws.InstanceOf<ArgumentOutOfRangeException>());
      Assert.That(() => list[1], Throws.InstanceOf<ArgumentOutOfRangeException>());
    });
  }

  [Test]
  public void Equals_Null_IsFalse()
  {
    // ARRANGE
    var list = new ValueList<int>(0xC0FFEE);

    // ASSERT
    Assert.That(list, Is.Not.EqualTo(null));
  }

  [Test]
  public void Equals_DifferentList_IsFalse()
  {
    // ARRANGE
    var list1 = new ValueList<int>(0xC0FFEE);
    var list2 = new ValueList<int>(0xBEEF);
    var list3 = new ValueList<int>(0xC0FFEE, 0xBEEF);
    var list4 = new ValueList<int>(ValueList<int>.Empty);

    // ASSERT
    Assert.Multiple(() =>
    {
      Assert.That(list1, Is.Not.EqualTo(list2));
      Assert.That(list1, Is.Not.EqualTo(list3));
      Assert.That(list1, Is.Not.EqualTo(list4));
      Assert.That(list2, Is.Not.EqualTo(list3));
      Assert.That(list2, Is.Not.EqualTo(list4));
      Assert.That(list3, Is.Not.EqualTo(list4));
    });
  }

  [Test]
  public void Equals_EqualLists_IsTrue()
  {
    // ARRANGE
    var list1 = new ValueList<int>(0xC0FFEE);
    var list2 = new ValueList<int>(0xC0FFEE);

    // ASSERT
    Assert.That(list1, Is.EqualTo(list2));
  }

  [Test]
  public void GetHashcode_ShouldReturn_ConsistentValue()
  {
    // ARRANGE
    var list1 = new ValueList<int>(0xC0FFEE);
    var list2 = new ValueList<int>(0xC0FFEE);

    // ACT
    var hash1 = list1.GetHashCode();
    var hash2 = list2.GetHashCode();

    // ASSERT
    Assert.That(hash1, Is.EqualTo(hash2));
  }

  private static IEnumerable<object[]> ValueListsAndCounts =>
    new List<object[]>
    {
      new object[] { new ValueList<int>(), 0 },
      new object[] { new ValueList<int>(1), 1 },
      new object[] { new ValueList<int>(1, 2, 3), 3 },
      new object[] { new ValueList<int>(1, 1, 1), 3 },
      new object[] { new ValueList<int?>(1, null), 2 },
    };
}