class TeamElement extends HTMLElement {
	constructor() {
		super()
	}

	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'open' });

		const teamInput = document.createElement('input');
		teamInput.setAttribute('type', 'text');
		teamInput.setAttribute('autocomplete', 'off');
		teamInput.setAttribute('placeholder', 'Team');
		shadow.appendChild(teamInput);

		const countersInput = document.createElement('input');
		countersInput.setAttribute('type', 'text');
		countersInput.setAttribute('autocomplete', 'off');
		countersInput.setAttribute('placeholder', 'Counters');
		shadow.appendChild(countersInput);
	}
}

customElements.define('team-info', TeamElement);

onload = (event) => {
	const typeSelect = document.querySelector('#type');
	const leagueSelect = document.querySelector('#league');

	typeSelect.addEventListener('change', (event) => {
		localStorage.setItem('type', typeSelect.value);

		updateTeams(typeSelect, leagueSelect);
	})

	leagueSelect.addEventListener('change', (event) => {
		localStorage.setItem('league', leagueSelect.value);

		updateTeams(typeSelect, leagueSelect);
	});

	const savedType = localStorage.getItem('type');
	if (savedType) {
		typeSelect.value = savedType;
	}

	const savedLeague = localStorage.getItem('league');
	if (savedLeague) {
		leagueSelect.value = savedLeague;
	}

	if (savedType || savedLeague) {
		updateTeams(typeSelect, leagueSelect);
	}
};

function updateTeams(typeSelect, leagueSelect) {
	const type = typeSelect.value;
	const league = leagueSelect.value;

	const zoneInfo = zoneSizes[type][league];
	const zones = document.querySelectorAll('.zone');
	for (let index = 0; index < zones.length; index++) {
		const zone = zones[index];
		const currentZoneInfo = zoneInfo[zone.id];

		removeAllChildren(zone);
		for (let teamIndex = 0; teamIndex < currentZoneInfo; teamIndex++) {
			const listItem = document.createElement('li');
			const team = document.createElement('team-info');
			listItem.appendChild(team);
			zone.appendChild(listItem);
		}
	}
}

function removeAllChildren(node) {
	while (node.firstChild) {
		node.removeChild(node.lastChild);
	}
}

const zoneSizes = {
	"3": {
		"carbonite": {
			"north": 1,
			"south": 1,
			"back": 1,
			"ships": 1
		},
		"bronzium": {
			"north": 2,
			"south": 2,
			"back": 3,
			"ships": 1
		},
		"chromium": {
			"north": 3,
			"south": 3,
			"back": 4,
			"ships": 2
		},
		"aurodium": {
			"north": 4,
			"south": 4,
			"back": 5,
			"ships": 2
		},
		"kyber": {
			"north": 5,
			"south": 5,
			"back": 5,
			"ships": 3
		}
	},
	"5": {
		"carbonite": {
			"north": 1,
			"south": 1,
			"back": 1,
			"ships": 1
		},
		"bronzium": {
			"north": 2,
			"south": 2,
			"back": 1,
			"ships": 1
		},
		"chromium": {
			"north": 3,
			"south": 2,
			"back": 2,
			"ships": 2
		},
		"aurodium": {
			"north": 3,
			"south": 3,
			"back": 3,
			"ships": 2
		},
		"kyber": {
			"north": 4,
			"south": 4,
			"back": 3,
			"ships": 3
		}
	}
};

