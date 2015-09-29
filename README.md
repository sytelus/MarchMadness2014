# March Madness Basketball Brackets Predictor
This was my entry for March Madeness basketball brackets prediction contest for 2014.

## Problem Statement
We have N teams competing with each other. We know some of the previous outcomes of few pairwise games for each team. We wish to predict of outcome future games for given pairs and for pairs of winners from those games until only one winner is left.

## Approach
This was designed, coded and tested in literally ~24 hours before the deadline for the contest, so no earth shattering ideas :). The main approach is to use pairwise ratings to rank each team so we get the total order. Then for given pairs, we can easily predict winners and final champion. This is a classic approach to classic problem and there have been several advances in the area. One of the famous but not-so-well-respected is Elo that is also used to rate chess players around the world. Another, perhaps the state of the art is TrueSkill that is being used for XBOX games for leaderboard. I chose the approach that can help me meet 24 hour deadline with more decency than Elo :). And that approach is called, not surprisingly, Elo++ developed by IBM researcher Yannis Sismanis that won the grand Kaggle competition to predict chess ratings.

## Elo++
Like Elo, Elo++ uses a single rating per player. It predicts the outcome of a
game, by using a logistic curve over the difference in ratings of the players. The major component of
Elo++ is a regularization technique that avoids overfitting. The dataset of chess games and outcomes
is relatively small and one has to be careful not to draw “too many conclusions” out of the limited data.
Overfitting seems to be a problem of many approaches tested in the competition. The leader-board of
the competition was dominated by attempts that did a very good job on a small test dataset, but couldn’t
generalize as well as Elo++ on the private hold-out dataset. The Elo++ regularization takes into account
the number of games per player, the recency of these games and the ratings of the opponents. Finally,
Elo++ employs a stochastic gradient descent scheme for training the ratings.


