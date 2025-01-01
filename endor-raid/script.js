const targetScores = [
    2700000,
    2000000,
    1400000,
    1400000,
    900000,
    600000,
    600000,
    600000
]

function computeScore() {
    const inputs = document.getElementsByClassName('raidScore');
    let score = 0;
    let targetScore = 0;
    for (let index = 0; index < inputs.length; index++) {
        const currentScore = Number(inputs[index].value);
        score += currentScore;

        if (currentScore !== 0) {
            targetScore += targetScores[index];
        }
    }

    return {
        score,
        targetScore
    };
}

function calculate() {
    const { score, targetScore } = computeScore();
    const aheadBehind = score - targetScore;

    document.getElementById('output').innerText = score.toLocaleString();

    const aheadBehindCtrl = document.getElementById('aheadBehind');
    aheadBehindCtrl.innerText = aheadBehind.toLocaleString();
    if (aheadBehind < 0) {
        aheadBehindCtrl.classList.add('behind');
    }
    else {
        aheadBehindCtrl.classList.remove('behind');
    }
}

function getSavedScores() {
    const savedScores = window.localStorage.getItem('scores');
    if (!savedScores) {
        return [];
    }

    return JSON.parse(savedScores);
}

function saveScores(scores) {
    const json = JSON.stringify(scores);
    window.localStorage.setItem('scores', json);
}

function saveScore() {
    const { score } = computeScore();

    const currentScore = {
        score: score,
        date: new Date()
    };

    let scores = getSavedScores();
    scores.push(currentScore);

    saveScores(scores);

    const inputs = document.getElementsByClassName('raidScore');
    for (let index = 0; index < inputs.length; index++) {
        inputs[index].value = '';
    }
    document.getElementById('aheadBehind').innerText = '';
    document.getElementById('output').innerText = '';
}
