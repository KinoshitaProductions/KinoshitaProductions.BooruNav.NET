using CommunityToolkit.Mvvm.Messaging;
using KinoshitaProductions.BooruNav.Presentation;
using KinoshitaProductions.BooruNav.ViewModels;
using Shouldly;
using Xunit;

namespace KinoshitaProductions.BooruNav.UnitTests;

public class WelcomeViewModelTests
{
    private static WelcomeViewModel Create(out IMessenger messenger)
    {
        messenger = new WeakReferenceMessenger();
        return new WelcomeViewModel(messenger);
    }

    [Fact]
    public void Starts_at_the_first_step()
    {
        var vm = Create(out _);

        vm.StepIndex.ShouldBe(0);
        vm.BackCommand.CanExecute(null).ShouldBeFalse();
        vm.NextCommand.CanExecute(null).ShouldBeTrue();
        vm.IsLastStep.ShouldBeFalse();
    }

    [Fact]
    public void Next_advances_and_back_returns()
    {
        var vm = Create(out _);

        vm.NextCommand.Execute(null);
        vm.StepIndex.ShouldBe(1);

        vm.BackCommand.Execute(null);
        vm.StepIndex.ShouldBe(0);
    }

    [Fact]
    public void Last_step_disables_next_and_is_marked_last()
    {
        var vm = Create(out _);

        vm.NextCommand.Execute(null); // → 1
        vm.NextCommand.Execute(null); // → 2 (last, of 3)

        vm.IsLastStep.ShouldBeTrue();
        vm.NextCommand.CanExecute(null).ShouldBeFalse();
    }

    [Fact]
    public void Finish_broadcasts_welcome_completed()
    {
        var vm = Create(out var messenger);
        var received = false;
        messenger.Register<WelcomeCompleted>(this, (_, _) => received = true);

        vm.FinishCommand.Execute(null);

        received.ShouldBeTrue();
    }

    [Fact]
    public void Skip_also_broadcasts_welcome_completed()
    {
        var vm = Create(out var messenger);
        var received = false;
        messenger.Register<WelcomeCompleted>(this, (_, _) => received = true);

        vm.SkipCommand.Execute(null);

        received.ShouldBeTrue();
    }
}
