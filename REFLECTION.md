# Reflection - Assignment 1

## Start

I'm going to start working on assignments the week they come out. This one caught me off guard. I'm doing an internship alongside school, and I assumed I could get this done in a day. I waited until the last minute and found out pretty fast that it wasn't a one day assignment. Between the 27 test cases, the rounding logic and the Git requirements, there was a lot more to it than I expected. I'm also going to start committing at least twice a week so my history actually shows the work happening over time instead of all at once.

## Stop

I need to stop treating the due date like the start date. I'm not using the internship as an excuse, because lots of people balance work and school. The real problem is that I underestimated the work and didn't plan for it. That cost me the Git cadence marks and a late penalty, and both were avoidable.

## Continue

I want to keep writing tests the way I did here, with a clear Arrange, Act, Assert layout and names that say exactly what's being checked. I'll also keep using decimal for anything involving money, and keep each class focused on one job, since that made the tests a lot easier to write. The two placeholder tests for CSV export and Stripe payments were a good way to see TDD in practice. You write the test first, watch it fail, and then build the feature to make it pass.

## What I learned

The biggest thing I took away is that testing is how you prove your code actually does what it's supposed to do. It's not enough for it to run. Every case that's supposed to work has to be checked, including the edge cases nobody thinks about. The penny reconciliation was a good example. Splitting $10.00 three ways looks simple until you notice $3.33 x 3 is $9.99 and a cent just disappeared. Without a test checking that the shares add back up to the total, that bug goes straight through.

I've seen the same thing at work. My current internship is on a data team, and if a number is off it's not just a bug, it's a real problem for whoever is relying on it. That's made me take testing a lot more seriously, and this assignment showed me how to build that into the code from the start instead of checking things by hand at the end.
