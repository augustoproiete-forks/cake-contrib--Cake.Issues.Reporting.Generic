﻿namespace Cake.Issues.Reporting.Generic.Tests
{
    using System.Collections.Generic;
    using Cake.Testing;
    using Shouldly;
    using Testing;
    using Xunit;

    public sealed class GenericIssueReportGeneratorTests
    {
        public sealed class TheCtor
        {
            [Fact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    new GenericIssueReportGenerator(
                        null,
                        GenericIssueReportFormatSettings.FromContent("Foo")));

                // Then
                result.IsArgumentNullException("log");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    new GenericIssueReportGenerator(
                        new FakeLog(),
                        null));

                // Then
                result.IsArgumentNullException("settings");
            }
        }

        public sealed class TheInternalCreateReportMethod
        {
            [Theory]
            [InlineData(GenericIssueReportTemplate.HtmlDiagnostic)]
            [InlineData(GenericIssueReportTemplate.HtmlDataTable)]
            public void Should_Generate_Report_From_Embedded_Template(GenericIssueReportTemplate template)
            {
                // Given
                var fixture = new GenericIssueReportFixture(template);
                var issues =
                    new List<IIssue>
                    {
                        IssueBuilder
                            .NewIssue("Message Foo", "ProviderType Foo", "ProviderName Foo")
                            .InFile(@"src\Cake.Issues.Reporting.Generic.Tests\Foo.cs", 10)
                            .OfRule("Rule Foo")
                            .WithPriority(IssuePriority.Warning)
                            .Create(),
                        IssueBuilder
                            .NewIssue("Message Bar", "ProviderType Bar", "ProviderName Bar")
                            .InFile(@"src\Cake.Issues.Reporting.Generic.Tests\Foo.cs", 12)
                            .OfRule("Rule Bar")
                            .WithPriority(IssuePriority.Warning)
                            .Create()
                        };

                // When
                var result = fixture.CreateReport(issues);

                // Then
                // No additional tests. We only check if template can be compiled.
            }

            [Fact]
            public void Should_Generate_Report_From_Custom_Template()
            {
                // Given
                var fixture = new GenericIssueReportFixture("<ul>@foreach(var issue in Model){<li>@issue.Message</li>}</ul>");
                var issues =
                    new List<IIssue>
                    {
                        IssueBuilder
                            .NewIssue("Message Foo", "ProviderType Foo", "ProviderName Foo")
                            .InFile(@"src\Cake.Issues.Reporting.Generic.Tests\Foo.cs", 10)
                            .OfRule("Rule Foo")
                            .WithPriority(IssuePriority.Warning)
                            .Create(),
                        IssueBuilder
                            .NewIssue("Message Bar", "ProviderType Bar", "ProviderName Bar")
                            .InFile(@"src\Cake.Issues.Reporting.Generic.Tests\Foo.cs", 12)
                            .OfRule("Rule Bar")
                            .WithPriority(IssuePriority.Warning)
                            .Create()
                        };
                var expectedResult =
                    @"<ul><li>Message Foo</li><li>Message Bar</li></ul>";

                // When
                var result = fixture.CreateReport(issues);

                // Then
                result.ShouldBe(expectedResult);
            }
        }
    }
}