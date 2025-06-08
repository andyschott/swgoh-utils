class TeamElement extends HTMLElement {
	constructor() {
		super()
	}

	infoChanged = new Event('info-changed');

	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'open' });

		const template = document.getElementById('team-info-template');
		shadow.appendChild(template.content.cloneNode(true));

		const team = shadow.getElementById('team');
		const counters = shadow.getElementById('counters');
		const complete = shadow.getElementById('complete');
		complete.addEventListener('change', (event) => {
			team.disabled = complete.checked;
			counters.disabled = complete.checked;
		});
		team.addEventListener('change', (event) => {
			this.dispatchEvent(this.infoChanged);
		});
		counters.addEventListener('change', (event) => {
			this.dispatchEvent(this.infoChanged);
		})
	}

	getInfo() {
		const shadow = this.shadowRoot;
		const team = shadow.getElementById('team');
		const counters = shadow.getElementById('counters');

		return {
			team: team.value,
			counters: counters.value
		}
	}

	setInfo(info) {
		const shadow = this.shadowRoot;
		const team = shadow.getElementById('team');
		const counters = shadow.getElementById('counters');

		team.value = info.team;
		counters.value = info.counters;
	}
}
customElements.define('team-info', TeamElement);

class ZoneElement extends HTMLElement {
	static observedAttributes = [
		'teams'
	];

	constructor() {
		super()
	}

	infoChanged = new Event('info-changed');

	connectedCallback() {
		const shadow = this.attachShadow({ mode: 'open' });

		const template = document.getElementById('zone-info-template');
		const instance = template.content.cloneNode(true);
		shadow.appendChild(instance);

		const expandDown = shadow.querySelector('#expandDown');
		const expandRight = shadow.querySelector('#expandRight');
		const teams = shadow.querySelector('#teams');

		expandDown.addEventListener('click', (event) => {
			expandDown.classList.add('hidden');
			expandRight.classList.remove('hidden');

			teams.classList.add('hidden');
		});

		expandRight.addEventListener('click', (event) => {
			expandDown.classList.remove('hidden');
			expandRight.classList.add('hidden');

			teams.classList.remove('hidden');
		});
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (name === 'teams') {
			const shadow = this.shadowRoot;
			const teamsContainer = shadow.querySelector('#teams');
			removeAllChildren(teamsContainer);

			for (let teamIndex = 0; teamIndex < newValue; teamIndex++) {
				const team = document.createElement('team-info');
				team.addEventListener('info-changed', (event) => {
					this.dispatchEvent(this.infoChanged);
				});
				teamsContainer.appendChild(team);
			}
		}
	}

	getInfo() {
		const shadow = this.shadowRoot;
		const teamsContainer = shadow.querySelector('#teams');
		const teams = teamsContainer.getElementsByTagName('team-info');
		const info = [];
		for (let index = 0; index < teams.length; index++) {
			const team = teams[index];
			info.push(team.getInfo());
		}
		return info;
	}

	setInfo(info) {
		const shadow = this.shadowRoot;
		const teamsContainer = shadow.querySelector('#teams');
		const teams = teamsContainer.getElementsByTagName('team-info');
		for (let index = 0; index < teams.length; index++) {
			const team = teams[index];
			const currentInfo = info[index];
			team.setInfo(currentInfo);
		}
	}
}
customElements.define('zone-info', ZoneElement);

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

	updateTeams(typeSelect, leagueSelect);
	restoreAllInfo();
};

function updateTeams(typeSelect, leagueSelect) {
	const type = typeSelect.value;
	const league = leagueSelect.value;

	const zoneInfo = zoneSizes[type][league];
	const zones = document.querySelectorAll('zone-info');
	for (let index = 0; index < zones.length; index++) {
		const zone = zones[index];
		zone.addEventListener('info-changed', (event) => {
			saveAllInfo();
		});
		zone.setAttribute('teams', zoneInfo[zone.id]);
	}
}

function removeAllChildren(node) {
	while (node.firstChild) {
		node.removeChild(node.lastChild);
	}
}

const MAX_INFO_AGE = 86400000; // 24 hours

function restoreAllInfo() {
	const rawInfo = window.localStorage.getItem('teams');
	if (!rawInfo) {
		return;
	}

	const info = JSON.parse(rawInfo);

	// If the saved info is more than 24 hours old, ignore it
	const now = new Date();
	const timestamp = new Date(info.timestamp);
	if (now.getTime() - timestamp.getTime() > MAX_INFO_AGE) {
		window.localStorage.removeItem('teams');
		return;
	}

	const zones = document.querySelectorAll('zone-info');
	for (let index = 0; index < zones.length; index++) {
		const zone = zones[index];
		const team = info.teams[index];
		zone.setInfo(team);
	}
}

function saveAllInfo() {
	const zones = document.querySelectorAll('zone-info');
	const teams = [];
	for (let index = 0; index < zones.length; index++) {
		const zone = zones[index];
		teams.push(zone.getInfo());
	}
	const info = {
		timestamp: new Date(),
		teams,
	};
	window.localStorage.setItem('teams', JSON.stringify(info));
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

