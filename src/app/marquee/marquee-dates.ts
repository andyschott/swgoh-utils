import { Injectable } from '@angular/core';

export interface MarqueeDate {
  name: string;
  introduction: Date;
  marqueeEvent: Date;
  shipment: Date;
  farm: Date;
  acceleration: Date | null;
}

const marquees: MarqueeDate[] = [
  {
    name: 'R5-D4',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-05-05'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-08-05'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Zeb Orrelios (New Republic Pilot)',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-05-19'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-08-19'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Colonel Ward',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-06-02'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-09-02'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Captain Carson Teva',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-06-16'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-09-16'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Grogu & Anzellans',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-06-30'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-06-30'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Imperial Snowtrooper Commander',
    introduction: new Date('2026-04-29'),
    marqueeEvent: new Date('2026-07-14'),
    shipment: new Date('2026-08-05'),
    farm: new Date('2026-10-14'),
    acceleration: new Date('2027-04-29')
  },
  {
    name: 'Sana Starros',
    introduction: new Date('2022-11-29'),
    marqueeEvent: new Date('2022-11-30'),
    shipment: new Date('2022-12-14'),
    farm: new Date('2023-01-25'),
    acceleration: new Date('2023-11-29')
  },
  {
    name: '0-0-0',
    introduction: new Date('2022-10-19'),
    marqueeEvent: new Date('2022-10-20'),
    shipment: new Date('2022-11-09'),
    farm: new Date('2022-12-14'),
    acceleration: new Date('2023-10-19')
  },
  {
    name: 'BT-1',
    introduction: new Date('2022-10-19'),
    marqueeEvent: new Date('2022-10-20'),
    shipment: new Date('2022-11-09'),
    farm: new Date('2022-12-14'),
    acceleration: new Date('2023-10-19')
  },
  {
    name: 'Hondo Ohnaka',
    introduction: new Date('2022-10-04'),
    marqueeEvent: new Date('2022-10-05'),
    shipment: new Date('2022-10-26'),
    farm: new Date('2022-11-30'),
    acceleration: new Date('2023-10-04')
  },
  {
    name: 'Boushh (Leia Organa)',
    introduction: new Date('2022-09-08'),
    marqueeEvent: new Date('2022-09-09'),
    shipment: new Date('2022-10-05'),
    farm: new Date('2022-11-09'),
    acceleration: new Date('2023-09-08')
  },
  {
    name: 'Skiff Guard (Lando Calrissian)',
    introduction: new Date('2022-08-17'),
    marqueeEvent: new Date('2022-08-18'),
    shipment: new Date('2022-09-07'),
    farm: new Date('2022-10-19'),
    acceleration: new Date('2023-08-17')
  },
  {
    name: 'Krrsantan',
    introduction: new Date('2022-07-20'),
    marqueeEvent: new Date('2022-07-21'),
    shipment: new Date('2022-08-17'),
    farm: new Date('2022-09-21'),
    acceleration: new Date('2023-07-20')
  },
  {
    name: 'Ravens Claw',
    introduction: new Date('2022-07-06'),
    marqueeEvent: new Date('2022-07-07'),
    shipment: new Date('2022-08-03'),
    farm: new Date('2022-09-07'),
    acceleration: null
  },
  {
    name: 'Admiral Raddus',
    introduction: new Date('2022-06-08'),
    marqueeEvent: new Date('2022-06-09'),
    shipment: new Date('2022-07-06'),
    farm: new Date('2022-08-03'),
    acceleration: new Date('2023-06-08')
  },
  {
    name: '50R-T',
    introduction: new Date('2022-05-11'),
    marqueeEvent: new Date('2022-05-12'),
    shipment: new Date('2022-06-08'),
    farm: new Date('2022-07-06'),
    acceleration: new Date('2023-05-11')
  },
  {
    name: 'Fifth Brother',
    introduction: new Date('2022-04-27'),
    marqueeEvent: new Date('2022-04-28'),
    shipment: new Date('2022-05-25'),
    farm: new Date('2022-06-08'),
    acceleration: new Date('2023-04-27')
  },
  {
    name: 'Outrider',
    introduction: new Date('2022-04-07'),
    marqueeEvent: new Date('2022-04-08'),
    shipment: new Date('2022-05-11'),
    farm: new Date('2022-06-08'),
    acceleration: null
  },
  {
    name: 'Eighth Brother',
    introduction: new Date('2022-03-23'),
    marqueeEvent: new Date('2022-03-24'),
    shipment: new Date('2022-04-20'),
    farm: new Date('2022-05-25'),
    acceleration: new Date('2023-03-23')
  },
  {
    name: 'Seventh Sister',
    introduction: new Date('2022-03-16'),
    marqueeEvent: new Date('2022-03-17'),
    shipment: new Date('2022-04-06'),
    farm: new Date('2022-05-11'),
    acceleration: new Date('2023-03-16')
  },
  {
    name: 'TIE Echelon',
    introduction: new Date('2022-03-03'),
    marqueeEvent: new Date('2022-03-04'),
    shipment: new Date('2022-04-06'),
    farm: new Date('2022-04-27'),
    acceleration: null
  },
  {
    name: 'MG-100 StarFortress SF-17',
    introduction: new Date('2022-02-24'),
    marqueeEvent: new Date('2022-02-25'),
    shipment: new Date('2022-03-23'),
    farm: new Date('2022-04-20'),
    acceleration: null
  },
  {
    name: 'Ninth Sister',
    introduction: new Date('2022-02-09'),
    marqueeEvent: new Date('2022-02-10'),
    shipment: new Date('2022-03-16'),
    farm: new Date('2022-04-06'),
    acceleration: new Date('2023-02-09')
  },
  {
    name: 'Second Sister',
    introduction: new Date('2022-01-26'),
    marqueeEvent: new Date('2022-01-27'),
    shipment: new Date('2022-02-23'),
    farm: new Date('2022-03-23'),
    acceleration: new Date('2023-01-26')
  },
  {
    name: 'Iden Versio',
    introduction: new Date('2022-01-12'),
    marqueeEvent: new Date('2022-01-13'),
    shipment: new Date('2022-02-09'),
    farm: new Date('2022-03-16'),
    acceleration: new Date('2023-01-12')
  },
  {
    name: 'Mara Jade, The Emperor\'s Hand',
    introduction: new Date('2021-12-08'),
    marqueeEvent: new Date('2021-12-09'),
    shipment: new Date('2021-01-12'),
    farm: new Date('2022-02-09'),
    acceleration: new Date('2022-12-08')
  },
  {
    name: 'Darth Talon',
    introduction: new Date('2021-12-01'),
    marqueeEvent: new Date('2021-12-02'),
    shipment: new Date('2021-01-12'),
    farm: new Date('2022-02-02'),
    acceleration: new Date('2022-12-01')
  },
  {
    name: 'Kyle Katarn',
    introduction: new Date('2021-11-24'),
    marqueeEvent: new Date('2021-11-25'),
    shipment: new Date('2021-12-15'),
    farm: new Date('2022-01-26'),
    acceleration: new Date('2022-11-24')
  },
  {
    name: 'Dash Rendar',
    introduction: new Date('2021-11-17'),
    marqueeEvent: new Date('2021-11-18'),
    shipment: new Date('2021-12-15'),
    farm: new Date('2021-01-12'),
    acceleration: new Date('2022-11-17')
  },
  {
    name: 'Fennec Shand',
    introduction: new Date('2021-10-06'),
    marqueeEvent: new Date('2021-10-07'),
    shipment: new Date('2021-11-03'),
    farm: new Date('2021-12-01'),
    acceleration: new Date('2022-10-06')
  },
  {
    name: 'Omega',
    introduction: new Date('2021-07-14'),
    marqueeEvent: new Date('2021-07-15'),
    shipment: new Date('2021-08-11'),
    farm: new Date('2021-09-08'),
    acceleration: new Date('2022-07-14')
  },
  {
    name: 'Echo',
    introduction: new Date('2021-05-04'),
    marqueeEvent: new Date('2021-05-05'),
    shipment: new Date('2021-06-09'),
    farm: new Date('2021-07-14'),
    acceleration: new Date('2022-05-04')
  },
  {
    name: 'Wrecker',
    introduction: new Date('2021-04-28'),
    marqueeEvent: new Date('2021-04-29'),
    shipment: new Date('2021-05-26'),
    farm: new Date('2021-06-30'),
    acceleration: new Date('2022-04-28')
  },
  {
    name: 'Tech',
    introduction: new Date('2021-04-14'),
    marqueeEvent: new Date('2021-04-15'),
    shipment: new Date('2021-05-19'),
    farm: new Date('2021-06-16'),
    acceleration: new Date('2022-04-14')
  },
  {
    name: 'Hunter',
    introduction: new Date('2021-04-07'),
    marqueeEvent: new Date('2021-04-08'),
    shipment: new Date('2021-05-04'),
    farm: new Date('2021-06-09'),
    acceleration: new Date('2022-04-07')
  },
  {
    name: 'Bo-Katan Kryze',
    introduction: new Date('2021-03-17'),
    marqueeEvent: new Date('2021-03-18'),
    shipment: new Date('2021-04-14'),
    farm: new Date('2021-05-19'),
    acceleration: new Date('2022-03-17')
  },
  {
    name: 'Dark Trooper',
    introduction: new Date('2021-03-03'),
    marqueeEvent: new Date('2021-03-04'),
    shipment: new Date('2021-04-05'),
    farm: new Date('2021-05-04'),
    acceleration: new Date('2022-03-03')
  },
  {
    name: 'The Armorer',
    introduction: new Date('2021-01-13'),
    marqueeEvent: new Date('2021-01-14'),
    shipment: new Date('2021-02-10'),
    farm: new Date('2021-03-17'),
    acceleration: new Date('2022-01-13')
  },
  {
    name: 'Moff Gideon',
    introduction: new Date('2020-12-02'),
    marqueeEvent: new Date('2020-12-03'),
    shipment: new Date('2021-01-06'),
    farm: new Date('2021-02-10'),
    acceleration: new Date('2021-12-02')
  },
  {
    name: 'Admiral Piett',
    introduction: new Date('2020-08-26'),
    marqueeEvent: new Date('2020-08-27'),
    shipment: new Date('2020-09-16'),
    farm: new Date('2020-10-07'),
    acceleration: new Date('2021-08-26')
  },
  {
    name: 'Mon Mothma',
    introduction: new Date('2020-07-26'),
    marqueeEvent: new Date('2020-07-27'),
    shipment: new Date('2020-08-26'),
    farm: new Date('2020-09-23'),
    acceleration: new Date('2021-07-26')
  },
  {
    name: 'Threepio & Chewie',
    introduction: new Date('2020-07-15'),
    marqueeEvent: new Date('2020-07-16'),
    shipment: new Date('2020-08-12'),
    farm: new Date('2020-09-09'),
    acceleration: new Date('2021-07-15')
  },
  {
    name: 'Imperial TIE Bomber',
    introduction: new Date('2020-06-03'),
    marqueeEvent: new Date('2020-06-04'),
    shipment: new Date('2020-07-09'),
    farm: new Date('2020-08-12'),
    acceleration: null
  },
  {
    name: 'Rebel Y-wing',
    introduction: new Date('2020-05-20'),
    marqueeEvent: new Date('2020-05-21'),
    shipment: new Date('2020-06-22'),
    farm: new Date('2020-07-23'),
    acceleration: null
  },
  {
    name: 'The Mandalorian',
    introduction: new Date('2020-04-29'),
    marqueeEvent: new Date('2020-04-30'),
    shipment: new Date('2020-06-03'),
    farm: new Date('2020-07-09'),
    acceleration: new Date('2021-04-29')
  },
  {
    name: 'Greef Karga',
    introduction: new Date('2020-04-26'),
    marqueeEvent: new Date('2020-04-27'),
    shipment: new Date('2020-06-03'),
    farm: new Date('2020-07-09'),
    acceleration: new Date('2021-04-26')
  },
  {
    name: 'Cara Dune',
    introduction: new Date('2020-04-22'),
    marqueeEvent: new Date('2020-04-23'),
    shipment: new Date('2020-06-03'),
    farm: new Date('2020-07-09'),
    acceleration: new Date('2021-04-22')
  },
  {
    name: 'Resistance Hero Poe',
    introduction: new Date('2020-01-22'),
    marqueeEvent: new Date('2020-01-23'),
    shipment: new Date('2020-03-10'),
    farm: new Date('2020-03-25'),
    acceleration: new Date('2021-01-22')
  },
  {
    name: 'Resistance Hero Finn',
    introduction: new Date('2019-12-18'),
    marqueeEvent: new Date('2019-12-19'),
    shipment: new Date('2020-01-29'),
    farm: new Date('2020-02-27'),
    acceleration: new Date('2020-12-18')
  },
  {
    name: 'Sith Trooper',
    introduction: new Date('2019-12-12'),
    marqueeEvent: new Date('2019-12-13'),
    shipment: new Date('2020-01-29'),
    farm: new Date('2020-02-12'),
    acceleration: new Date('2020-12-12')
  },
  {
    name: 'General Hux',
    introduction: new Date('2019-12-10'),
    marqueeEvent: new Date('2019-12-11'),
    shipment: new Date('2020-01-29'),
    farm: new Date('2020-02-12'),
    acceleration: new Date('2020-12-10')
  },
  {
    name: 'BTL-B Y-wing Starfighter',
    introduction: new Date('2019-11-13'),
    marqueeEvent: new Date('2019-11-14'),
    shipment: new Date('2019-12-18'),
    farm: new Date('2020-01-29'),
    acceleration: null
  },
  {
    name: 'Hyena Bomber',
    introduction: new Date('2019-10-31'),
    marqueeEvent: new Date('2019-11-01'),
    shipment: new Date('2019-12-10'),
    farm: new Date('2020-01-22'),
    acceleration: null
  },
  {
    name: 'Vulture Droid',
    introduction: new Date('2019-08-01'),
    marqueeEvent: new Date('2019-08-02'),
    shipment: new Date('2019-09-11'),
    farm: new Date('2019-10-30'),
    acceleration: null
  },
  {
    name: 'Shaak Ti',
    introduction: new Date('2019-06-05'),
    marqueeEvent: new Date('2019-06-06'),
    shipment: new Date('2019-07-16'),
    farm: new Date('2019-08-23'),
    acceleration: new Date('2020-06-05')
  },
  {
    name: 'Ebon Hawk',
    introduction: new Date('2019-03-07'),
    marqueeEvent: new Date('2019-03-08'),
    shipment: new Date('2019-04-11'),
    farm: new Date('2019-05-15'),
    acceleration: new Date('2020-03-07')
  },
  {
    name: 'Droideka',
    introduction: new Date('2019-02-20'),
    marqueeEvent: new Date('2019-02-21'),
    shipment: new Date('2019-03-27'),
    farm: new Date('2019-04-24'),
    acceleration: new Date('2020-02-20')
  },
  {
    name: 'B1 Battle Droid',
    introduction: new Date('2019-02-13'),
    marqueeEvent: new Date('2019-02-14'),
    shipment: new Date('2019-03-25'),
    farm: new Date('2019-04-24'),
    acceleration: new Date('2020-02-13')
  },
  {
    name: 'Emperors Shuttle',
    introduction: new Date('2019-02-07'),
    marqueeEvent: new Date('2019-02-08'),
    shipment: new Date('2019-03-25'),
    farm: new Date('2019-05-01'),
    acceleration: null
  },
  {
    name: 'Carth Onasi',
    introduction: new Date('2018-11-30'),
    marqueeEvent: new Date('2018-12-01'),
    shipment: new Date('2019-01-09'),
    farm: new Date('2019-02-20'),
    acceleration: new Date('2019-11-30')
  },
  {
    name: 'Juhani',
    introduction: new Date('2018-11-29'),
    marqueeEvent: new Date('2018-11-30'),
    shipment: new Date('2019-01-09'),
    farm: new Date('2019-02-20'),
    acceleration: new Date('2019-11-29')
  },
  {
    name: 'Canderous Ordo',
    introduction: new Date('2018-11-15'),
    marqueeEvent: new Date('2018-11-16'),
    shipment: new Date('2018-12-13'),
    farm: new Date('2019-01-16'),
    acceleration: new Date('2019-11-15')
  },
  {
    name: 'Bastila Shan (Fallen)',
    introduction: new Date('2018-11-14'),
    marqueeEvent: new Date('2018-11-15'),
    shipment: new Date('2018-12-13'),
    farm: new Date('2019-01-16'),
    acceleration: new Date('2019-11-14')
  },
  {
    name: 'Eta-2 Starfighter',
    introduction: new Date('2018-10-31'),
    marqueeEvent: new Date('2018-11-01'),
    shipment: new Date('2018-12-05'),
    farm: new Date('2019-01-16'),
    acceleration: new Date('2019-10-31')
  },
  {
    name: 'B-28 Extinction-class Bomber',
    introduction: new Date('2018-10-24'),
    marqueeEvent: new Date('2018-10-25'),
    shipment: new Date('2018-11-29'),
    farm: new Date('2019-01-09'),
    acceleration: null
  },
  {
    name: 'IG-2000',
    introduction: new Date('2018-09-27'),
    marqueeEvent: new Date('2018-09-28'),
    shipment: new Date('2018-10-31'),
    farm: new Date('2018-12-05'),
    acceleration: null
  },
  {
    name: 'Jango Fett',
    introduction: new Date('2018-09-19'),
    marqueeEvent: new Date('2018-09-20'),
    shipment: new Date('2018-10-31'),
    farm: new Date('2018-12-05'),
    acceleration: new Date('2019-09-19')
  },
  {
    name: 'Xanadu Blood Ship',
    introduction: new Date('2018-09-12'),
    marqueeEvent: new Date('2018-09-13'),
    shipment: new Date('2018-10-17'),
    farm: new Date('2018-11-29'),
    acceleration: null
  },
  {
    name: 'Aurra Sing',
    introduction: new Date('2018-08-23'),
    marqueeEvent: new Date('2018-08-24'),
    shipment: new Date('2018-09-26'),
    farm: new Date('2018-10-31'),
    acceleration: new Date('2019-08-23')
  },
  {
    name: 'Hounds Tooth',
    introduction: new Date('2018-08-22'),
    marqueeEvent: new Date('2018-08-23'),
    shipment: new Date('2018-09-26'),
    farm: new Date('2018-10-31'),
    acceleration: null
  },
  {
    name: 'Embo',
    introduction: new Date('2018-08-08'),
    marqueeEvent: new Date('2018-08-09'),
    shipment: new Date('2018-09-12'),
    farm: new Date('2018-10-17'),
    acceleration: new Date('2019-08-08')
  },
  {
    name: 'Zaalbar',
    introduction: new Date('2018-07-26'),
    marqueeEvent: new Date('2018-07-27'),
    shipment: new Date('2018-08-30'),
    farm: new Date('2018-09-26'),
    acceleration: new Date('2019-07-26')
  },
  {
    name: 'Mission Vao',
    introduction: new Date('2018-07-26'),
    marqueeEvent: new Date('2018-07-27'),
    shipment: new Date('2018-08-30'),
    farm: new Date('2018-09-26'),
    acceleration: new Date('2019-07-26')
  },
  {
    name: 'Sith Fighter',
    introduction: new Date('2018-07-25'),
    marqueeEvent: new Date('2018-07-26'),
    shipment: new Date('2018-08-30'),
    farm: new Date('2018-10-10'),
    acceleration: null
  },
  {
    name: 'T3-M4',
    introduction: new Date('2018-07-25'),
    marqueeEvent: new Date('2018-07-26'),
    shipment: new Date('2018-08-30'),
    farm: new Date('2018-09-26'),
    acceleration: new Date('2019-07-25')
  },
  {
    name: 'Jolee Bindo',
    introduction: new Date('2018-07-12'),
    marqueeEvent: new Date('2018-07-13'),
    shipment: new Date('2018-08-09'),
    farm: new Date('2018-09-12'),
    acceleration: new Date('2019-07-12')
  },
  {
    name: 'Bastila Shan',
    introduction: new Date('2018-07-11'),
    marqueeEvent: new Date('2018-07-12'),
    shipment: new Date('2018-08-09'),
    farm: new Date('2018-09-12'),
    acceleration: new Date('2019-07-11')
  },
  {
    name: 'Range Trooper',
    introduction: new Date('2018-06-21'),
    marqueeEvent: new Date('2018-06-22'),
    shipment: new Date('2018-07-25'),
    farm: new Date('2018-08-22'),
    acceleration: new Date('2019-06-21')
  },
  {
    name: 'Lando\'s Millennium Falcon',
    introduction: new Date('2018-06-06'),
    marqueeEvent: new Date('2018-06-07'),
    shipment: new Date('2018-07-25'),
    farm: new Date('2018-08-22'),
    acceleration: null
  },
  {
    name: 'L3-37',
    introduction: new Date('2018-06-06'),
    marqueeEvent: new Date('2018-06-07'),
    shipment: new Date('2018-07-25'),
    farm: new Date('2018-08-22'),
    acceleration: new Date('2019-06-06')
  },
  {
    name: 'Young Lando Calrissian',
    introduction: new Date('2018-06-06'),
    marqueeEvent: new Date('2018-06-07'),
    shipment: new Date('2018-07-25'),
    farm: new Date('2018-08-22'),
    acceleration: new Date('2019-06-06')
  },
  {
    name: 'Enfys Nest',
    introduction: new Date('2018-05-29'),
    marqueeEvent: new Date('2018-05-30'),
    shipment: new Date('2018-07-11'),
    farm: new Date('2018-08-22'),
    acceleration: new Date('2019-05-29')
  },
  {
    name: 'Qi\'ra',
    introduction: new Date('2018-05-24'),
    marqueeEvent: new Date('2018-05-25'),
    shipment: new Date('2018-07-11'),
    farm: new Date('2018-08-09'),
    acceleration: new Date('2019-05-24')
  },
  {
    name: 'Vandor Chewbacca',
    introduction: new Date('2018-05-16'),
    marqueeEvent: new Date('2018-05-17'),
    shipment: new Date('2018-06-21'),
    farm: new Date('2018-07-25'),
    acceleration: new Date('2019-05-16')
  },
  {
    name: 'Young Han Solo',
    introduction: new Date('2018-05-16'),
    marqueeEvent: new Date('2018-05-17'),
    shipment: new Date('2018-06-21'),
    farm: new Date('2018-07-25'),
    acceleration: new Date('2019-05-16')
  },
  {
    name: 'Bossk',
    introduction: new Date('2018-05-03'),
    marqueeEvent: new Date('2018-05-04'),
    shipment: new Date('2018-06-07'),
    farm: new Date('2018-07-11'),
    acceleration: new Date('2019-05-03')
  },
  {
    name: 'Darth Sion',
    introduction: new Date('2018-02-28'),
    marqueeEvent: new Date('2018-03-01'),
    shipment: new Date('2018-04-11'),
    farm: new Date('2018-05-23'),
    acceleration: new Date('2019-02-28')
  },
  {
    name: 'Visas Marr',
    introduction: new Date('2018-02-28'),
    marqueeEvent: new Date('2018-03-01'),
    shipment: new Date('2018-04-11'),
    farm: new Date('2018-05-23'),
    acceleration: new Date('2019-02-28')
  },
  {
    name: 'Sith Marauder',
    introduction: new Date('2018-02-13'),
    marqueeEvent: new Date('2018-02-14'),
    shipment: new Date('2018-04-11'),
    farm: new Date('2018-05-14'),
    acceleration: new Date('2019-02-13')
  },
  {
    name: 'Amilyn Holdo',
    introduction: new Date('2018-01-26'),
    marqueeEvent: new Date('2018-01-27'),
    shipment: new Date('2018-03-21'),
    farm: new Date('2018-04-25'),
    acceleration: new Date('2019-01-26')
  },
  {
    name: 'Rose Tico',
    introduction: new Date('2018-01-26'),
    marqueeEvent: new Date('2018-01-27'),
    shipment: new Date('2018-03-21'),
    farm: new Date('2018-04-25'),
    acceleration: new Date('2019-01-26')
  },
  {
    name: 'First Order Executioner',
    introduction: new Date('2018-01-10'),
    marqueeEvent: new Date('2018-01-11'),
    shipment: new Date('2018-03-01'),
    farm: new Date('2018-04-11'),
    acceleration: new Date('2019-01-10')
  },
  {
    name: 'Kylo Ren (Unmasked)',
    introduction: new Date('2017-11-08'),
    marqueeEvent: new Date('2017-11-09'),
    shipment: new Date('2017-12-14'),
    farm: new Date('2018-01-25'),
    acceleration: new Date('2018-11-08')
  },
  {
    name: 'TIE Silencer',
    introduction: new Date('2017-11-08'),
    marqueeEvent: new Date('2017-11-09'),
    shipment: new Date('2017-12-14'),
    farm: new Date('2018-01-25'),
    acceleration: null
  },
  {
    name: 'First Order SF Tie Fighter',
    introduction: new Date('2017-11-08'),
    marqueeEvent: new Date('2017-11-09'),
    shipment: new Date('2017-12-14'),
    farm: new Date('2018-01-25'),
    acceleration: null
  },
  {
    name: 'First Order SF Tie Pilot',
    introduction: new Date('2017-11-08'),
    marqueeEvent: new Date('2017-11-09'),
    shipment: new Date('2017-12-14'),
    farm: new Date('2018-01-25'),
    acceleration: null
  },
  {
    name: 'Mother Talzin',
    introduction: new Date('2017-10-19'),
    marqueeEvent: new Date('2017-10-20'),
    shipment: new Date('2018-01-23'),
    farm: new Date('2018-04-11'),
    acceleration: new Date('2018-10-19')
  },
  {
    name: 'Zorii Bliss',
    introduction: new Date('2023-01-11'),
    marqueeEvent: new Date('2023-01-12'),
    shipment: new Date('2023-02-08'),
    farm: new Date('2023-03-08'),
    acceleration: new Date('2024-01-11'),
  },
  {
    name: 'Tie Defender',
    introduction: new Date('2023-01-26'),
    marqueeEvent: new Date('2023-01-27'),
    shipment: new Date('2023-02-22'),
    farm: new Date('2023-03-22'),
    acceleration: null
  },
  {
    name: 'Tusken Chieftan',
    introduction: new Date('2023-02-08'),
    marqueeEvent: new Date('2023-02-09'),
    shipment: new Date('2023-03-08'),
    farm: new Date('2023-04-05'),
    acceleration: new Date('2024-02-08'),
  },
  {
    name: 'Tusken Warrior',
    introduction: new Date('2023-02-22'),
    marqueeEvent: new Date('2023-02-23'),
    shipment: new Date('2023-03-22'),
    farm: new Date('2023-04-19'),
    acceleration: new Date('2024-02-22'),
  },
  {
    name: 'Cal Kestis',
    introduction: new Date('2023-03-08'),
    marqueeEvent: new Date('2023-03-09'),
    shipment: new Date('2023-04-05'),
    farm: new Date('2023-05-03'),
    acceleration: new Date('2024-03-08'),
  },
  {
    name: 'Cere Junda',
    introduction: new Date('2023-03-22'),
    marqueeEvent: new Date('2023-03-23'),
    shipment: new Date('2023-04-19'),
    farm: new Date('2023-05-24'),
    acceleration: new Date('2024-03-22'),
  },
  {
    name: 'Merrin',
    introduction: new Date('2023-04-05'),
    marqueeEvent: new Date('2023-04-06'),
    shipment: new Date('2023-05-03'),
    farm: new Date('2023-06-07'),
    acceleration: new Date('2024-04-05'),
  },
  {
    name: 'Tarfful',
    introduction: new Date('2023-04-26'),
    marqueeEvent: new Date('2023-04-27'),
    shipment: new Date('2023-05-24'),
    farm: new Date('2023-06-07'),
    acceleration: new Date('2024-04-26'),
  },
  {
    name: 'Saw Gerrara',
    introduction: new Date('2023-05-02'),
    marqueeEvent: new Date('2023-05-03'),
    shipment: new Date('2023-05-24'),
    farm: new Date('2023-06-07'),
    acceleration: new Date('2024-05-02'),
  },
  {
    name: 'Mark VI Interceptor',
    introduction: new Date('2023-06-08'),
    marqueeEvent: new Date('2023-06-09'),
    shipment: new Date('2023-07-06'),
    farm: new Date('2023-07-19'),
    acceleration: null
  },
  {
    name: 'TIE Dagger',
    introduction: new Date('2023-05-25'),
    marqueeEvent: new Date('2023-05-26'),
    shipment: new Date('2023-06-21'),
    farm: new Date('2023-07-19'),
    acceleration: null
  },
  {
    name: 'Captain Rex',
    introduction: new Date('2023-06-20'),
    marqueeEvent: new Date('2023-06-21'),
    shipment: new Date('2023-07-19'),
    farm: new Date('2023-08-09'),
    acceleration: new Date('2024-06-20'),
  },
  {
    name: 'Princess Kneesaa',
    introduction: new Date('2023-07-11'),
    marqueeEvent: new Date('2023-07-12'),
    shipment: new Date('2023-08-09'),
    farm: new Date('2023-09-13'),
    acceleration: new Date('2024-07-11'),
  },
  {
    name: 'Scout Trooper',
    introduction: new Date('2023-08-08'),
    marqueeEvent: new Date('2023-08-09'),
    shipment: new Date('2023-09-13'),
    farm: new Date('2023-10-04'),
    acceleration: new Date('2024-08-08'),
  },
  {
    name: 'Captain Drogan',
    introduction: new Date('2023-08-30'),
    marqueeEvent: new Date('2023-08-31'),
    shipment: new Date('2023-09-27'),
    farm: new Date('2023-10-25'),
    acceleration: new Date('2024-08-30'),
  },
  {
    name: 'Maruader',
    introduction: new Date('2023-10-05'),
    marqueeEvent: new Date('2023-10-06'),
    shipment: new Date('2023-11-08'),
    farm: new Date('2023-12-08'),
    acceleration: null,
  },
  {
    name: 'Paz Vizsla',
    introduction: new Date('2023-10-17'),
    marqueeEvent: new Date('2023-10-18'),
    shipment: new Date('2023-11-22'),
    farm: new Date('2023-12-20'),
    acceleration: new Date('2024-10-17'),
  },
  {
    name: 'IG12 & Grogu',
    introduction: new Date('2023-11-15'),
    marqueeEvent: new Date('2023-11-16'),
    shipment: new Date('2023-12-21'),
    farm: new Date('2024-01-18'),
    acceleration: new Date('2024-11-15'),
  },
  {
    name: 'Kelleran Beq',
    introduction: new Date('2023-12-05'),
    marqueeEvent: new Date('2023-12-06'),
    shipment: new Date('2023-12-20'),
    farm: new Date('2024-02-07'),
    acceleration: new Date('2024-12-06'),
  },
  {
    name: 'Comeuppance',
    introduction: new Date('2024-01-15'),
    marqueeEvent: new Date('2024-01-16'),
    shipment: new Date('2024-02-07'),
    farm: new Date('2024-03-06'),
    acceleration: null,
  },
  {
    name: 'STAP',
    introduction: new Date('2024-02-06'),
    marqueeEvent: new Date('2024-02-07'),
    shipment: new Date('2024-03-06'),
    farm: new Date('2024-04-10'),
    acceleration: new Date('2025-02-07'),
  },
  {
    name: 'Boss Nass',
    introduction: new Date('2024-02-21'),
    marqueeEvent: new Date('2024-02-22'),
    shipment: new Date('2024-03-28'),
    farm: new Date('2024-04-24'),
    acceleration: new Date('2025-02-22'),
  },
  {
    name: 'Captain Tarpals',
    introduction: new Date('2024-03-05'),
    marqueeEvent: new Date('2024-03-06'),
    shipment: new Date('2024-04-10'),
    farm: new Date('2024-05-08'),
    acceleration: new Date('2025-03-06'),
  },
  {
    name: 'Gungan Boomadier',
    introduction: new Date('2024-03-26'),
    marqueeEvent: new Date('2024-03-27'),
    shipment: new Date('2024-04-24'),
    farm: new Date('2024-05-22'),
    acceleration: new Date('2025-03-27'),
  },
  {
    name: 'Gungan Phalanx',
    introduction: new Date('2024-04-09'),
    marqueeEvent: new Date('2024-04-10'),
    shipment: new Date('2024-05-08'),
    farm: new Date('2024-06-05'),
    acceleration: new Date('2025-04-10'),
  },
  {
    name: 'Padawan Obi-Wan',
    introduction: new Date('2024-05-07'),
    marqueeEvent: new Date('2024-05-08'),
    shipment: new Date('2024-06-05'),
    farm: new Date('2024-07-02'),
    acceleration: new Date('2025-05-08'),
  },
  {
    name: 'Master Qui-Gon',
    introduction: new Date('2024-05-21'),
    marqueeEvent: new Date('2024-05-22'),
    shipment: new Date('2024-06-19'),
    farm: new Date('2024-07-17'),
    acceleration: new Date('2025-05-22'),
  },
  {
    name: 'Night Trooper',
    introduction: new Date('2024-07-01'),
    marqueeEvent: new Date('2024-07-02'),
    shipment: new Date('2024-07-31'),
    farm: new Date('2024-08-28'),
    acceleration: new Date('2025-07-02'),
  },
  {
    name: 'Morgan Elsbeth',
    introduction: new Date('2024-07-16'),
    marqueeEvent: new Date('2024-07-17'),
    shipment: new Date('2024-08-14'),
    farm: new Date('2024-09-18'),
    acceleration: new Date('2025-07-17'),
  },
  {
    name: 'Death Trooper (Peridea)',
    introduction: new Date('2024-07-30'),
    marqueeEvent: new Date('2024-07-31'),
    shipment: new Date('2024-08-28'),
    farm: new Date('2024-10-02'),
    acceleration: new Date('2025-07-31'),
  },
  {
    name: 'Captain Enoch',
    introduction: new Date('2024-08-13'),
    marqueeEvent: new Date('2024-08-14'),
    shipment: new Date('2024-09-18'),
    farm: new Date('2024-10-16'),
    acceleration: new Date('2025-08-14'),
  },
  {
    name: 'Great Mothers',
    introduction: new Date('2024-09-03'),
    marqueeEvent: new Date('2024-09-04'),
    shipment: new Date('2024-10-02'),
    farm: new Date('2024-11-06'),
    acceleration: new Date('2025-09-04'),
  },
  {
    name: 'Shin Hati',
    introduction: new Date('2024-09-17'),
    marqueeEvent: new Date('2024-09-18'),
    shipment: new Date('2024-10-16'),
    farm: new Date('2024-11-20'),
    acceleration: new Date('2025-09-18'),
  },
  {
    name: 'Marrok',
    introduction: new Date('2024-10-01'),
    marqueeEvent: new Date('2024-10-02'),
    shipment: new Date('2024-11-06'),
    farm: new Date('2024-12-04'),
    acceleration: new Date('2025-10-02'),
  },
  {
    name: 'Punishing One',
    introduction: new Date('2024-10-15'),
    marqueeEvent: new Date('2024-10-16'),
    shipment: new Date('2024-11-20'),
    farm: new Date('2024-12-18'),
    acceleration: null,
  },
  {
    name: 'Padawan Sabine Wren',
    introduction: new Date('2024-11-05'),
    marqueeEvent: new Date('2024-11-06'),
    shipment: new Date('2024-12-02'),
    farm: new Date('2025-01-13'),
    acceleration: new Date('2025-11-06'),
  },
  {
    name: 'Huyang',
    introduction: new Date('2024-11-17'),
    marqueeEvent: new Date('2024-11-18'),
    shipment: new Date('2024-12-16'),
    farm: new Date('2025-01-22'),
    acceleration: new Date('2025-11-18'),
  },
  {
    name: 'General Syndulla',
    introduction: new Date('2024-12-01'),
    marqueeEvent: new Date('2024-12-02'),
    shipment: new Date('2025-01-13'),
    farm: new Date('2025-02-05'),
    acceleration: new Date('2025-12-02'),
  },
  {
    name: 'Omega (Fugitive)',
    introduction: new Date('2025-01-07'),
    marqueeEvent: new Date('2025-01-13'),
    shipment: new Date('2025-02-05'),
    farm: new Date('2025-03-19'),
    acceleration: new Date('2026-01-13'),
  },
  {
    name: 'Batcher',
    introduction: new Date('2025-01-16'),
    marqueeEvent: new Date('2025-01-22'),
    shipment: new Date('2025-02-19'),
    farm: new Date('2025-04-02'),
    acceleration: new Date('2026-01-22'),
  },
  {
    name: 'Hunter (Mercenary)',
    introduction: new Date('2025-01-30'),
    marqueeEvent: new Date('2025-02-05'),
    shipment: new Date('2025-03-19'),
    farm: new Date('2025-04-16'),
    acceleration: new Date('2026-02-05'),
  },
  {
    name: 'Wrecker (Mercenary)',
    introduction: new Date('2025-02-13'),
    marqueeEvent: new Date('2025-02-19'),
    shipment: new Date('2025-03-19'),
    farm: new Date('2025-04-30'),
    acceleration: new Date('2026-02-19'),
  },
  {
    name: 'Crosshair (Scarred)',
    introduction: new Date('2025-02-27'),
    marqueeEvent: new Date('2025-03-05'),
    shipment: new Date('2025-04-02'),
    farm: new Date('2025-05-14'),
    acceleration: new Date('2026-03-05'),
  },
  {
    name: 'CC-1119 "Appo"',
    introduction: new Date('2025-03-27'),
    marqueeEvent: new Date('2025-04-02'),
    shipment: new Date('2025-04-30'),
    farm: new Date('2025-06-11'),
    acceleration: new Date('2026-04-02'),
  },
  {
    name: 'CX-2',
    introduction: new Date('2025-04-10'),
    marqueeEvent: new Date('2025-04-16'),
    shipment: new Date('2025-05-28'),
    farm: new Date('2025-06-25'),
    acceleration: new Date('2026-04-16'),
  },
  {
    name: 'Disguised Clone Trooper',
    introduction: new Date('2025-04-30'),
    marqueeEvent: new Date('2025-05-06'),
    shipment: new Date('2025-06-11'),
    farm: new Date('2025-07-09'),
    acceleration: new Date('2026-04-30'),
  },
  {
    name: 'RC-1262 "Scorch"',
    introduction: new Date('2025-05-14'),
    marqueeEvent: new Date('2025-05-20'),
    shipment: new Date('2025-06-25'),
    farm: new Date('2025-07-23'),
    acceleration: new Date('2026-05-14'),
  },
  {
    name: 'Temple Guard',
    introduction: new Date('2025-05-28'),
    marqueeEvent: new Date('2025-06-03'),
    shipment: new Date('2025-07-09'),
    farm: new Date('2025-08-06'),
    acceleration: new Date('2026-05-28'),
  },
  {
    name: 'Depa Billaba',
    introduction: new Date('2025-06-11'),
    marqueeEvent: new Date('2025-06-17'),
    shipment: new Date('2025-07-23'),
    farm: new Date('2025-08-20'),
    acceleration: new Date('2026-06-11'),
  },
  {
    name: 'Captain Ithano',
    introduction: new Date('2025-06-25'),
    marqueeEvent: new Date('2025-07-01'),
    shipment: new Date('2025-08-06'),
    farm: new Date('2025-09-03'),
    acceleration: new Date('2026-06-25'),
  },
  {
    name: 'Quiggold',
    introduction: new Date('2025-07-09'),
    marqueeEvent: new Date('2025-07-15'),
    shipment: new Date('2025-08-20'),
    farm: new Date('2025-09-17'),
    acceleration: new Date('2026-07-09'),
  },
  {
    name: 'Kix',
    introduction: new Date('2025-07-23'),
    marqueeEvent: new Date('2025-07-29'),
    shipment: new Date('2025-09-03'),
    farm: new Date('2025-10-01'),
    acceleration: new Date('2026-07-23'),
  },
  {
    name: 'Brutus',
    introduction: new Date('2025-08-06'),
    marqueeEvent: new Date('2025-08-12'),
    shipment: new Date('2025-09-17'),
    farm: new Date('2025-10-15'),
    acceleration: new Date('2026-08-06'),
  },
  {
    name: 'Vane',
    introduction: new Date('2025-08-20'),
    marqueeEvent: new Date('2025-08-26'),
    shipment: new Date('2025-10-01'),
    farm: new Date('2025-10-29'),
    acceleration: new Date('2026-08-20'),
  },
  {
    name: 'Captain Silvo',
    introduction: new Date('2025-09-03'),
    marqueeEvent: new Date('2025-09-09'),
    shipment: new Date('2025-10-15'),
    farm: new Date('2025-11-18'),
    acceleration: new Date('2026-09-03'),
  },
  {
    name: 'Rogue One',
    introduction: new Date('2025-09-17'),
    marqueeEvent: new Date('2025-09-23'),
    shipment: new Date('2025-10-29'),
    farm: new Date('2025-12-10'),
    acceleration: null,
  },
  {
    name: 'Rebel B-Wing',
    introduction: new Date('2025-10-01'),
    marqueeEvent: new Date('2025-10-07'),
    shipment: new Date('2025-11-12'),
    farm: new Date('2026-01-07'),
    acceleration: null,
  },
  {
    name: '4-LOM',
    introduction: new Date('2025-10-15'),
    marqueeEvent: new Date('2025-10-21'),
    shipment: new Date('2025-11-25'),
    farm: new Date('2026-01-07'),
    acceleration: new Date('2026-10-15'),
  },
  {
    name: 'Zuckuss',
    introduction: new Date('2025-10-29'),
    marqueeEvent: new Date('2025-11-04'),
    shipment: new Date('2025-12-10'),
    farm: new Date('2026-01-07'),
    acceleration: new Date('2026-10-29'),
  },
  {
    name: 'Stormtrooper Luke',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2025-11-18'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-02-18'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'IG-90',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2025-12-02'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-03-04'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'Yoda & Chewie',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2025-12-16'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-03-18'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'Asajj Ventress (Dark Disciple)',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2025-12-30'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-04-01'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'Inquisitor Barriss',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2026-01-13'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-04-15'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'Darth Vader (Duel\'s End)',
    introduction: new Date('2025-11-18'),
    marqueeEvent: new Date('2026-01-27'),
    shipment: new Date('2026-02-18'),
    farm: new Date('2026-04-29'),
    acceleration: new Date('2026-11-18'),
  },
  {
    name: 'Kleya Marki',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-02-10'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-05-13'),
    acceleration: new Date('2027-02-04'),
  },
  {
    name: 'Vel Sartha',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-02-24'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-05-27'),
    acceleration: new Date('2027-02-04'),
  },
  {
    name: 'Cinta Kaz',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-03-10'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-06-10'),
    acceleration: new Date('2027-02-04'),
  },
  {
    name: 'Dedra Meero',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-03-24'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-06-24'),
    acceleration: new Date('2027-02-04'),
  },
  {
    name: 'Major Partagaz',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-04-07'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-07-08'),
    acceleration: new Date('2027-02-04'),
  },
  {
    name: 'KX Droid',
    introduction: new Date('2026-02-04'),
    marqueeEvent: new Date('2026-04-21'),
    shipment: new Date('2026-05-13'),
    farm: new Date('2026-07-22'),
    acceleration: new Date('2027-02-04'),
  },
];

@Injectable({
  providedIn: 'root',
})
export class MarqueeDates {
  getNames(): ReadonlyArray<string> {
    return marquees.map((marquee) => marquee.name);
  }

  getDates(name: string): MarqueeDate | null {
    const match = marquees.find((marquee) => marquee.name === name);

    if (!match) {
      return null;
    }

    return match;
  }
}
