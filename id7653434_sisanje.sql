-- phpMyAdmin SQL Dump
-- version 4.7.7
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jan 15, 2019 at 10:32 PM
-- Server version: 10.1.30-MariaDB
-- PHP Version: 7.2.2

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `id7653434_sisanje`
--

-- --------------------------------------------------------

--
-- Table structure for table `korisnik`
--

CREATE TABLE `korisnik` (
  `rfid` varchar(45) NOT NULL,
  `slika` varchar(45) NOT NULL,
  `ime` varchar(45) NOT NULL,
  `prezime` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `korisnik`
--

INSERT INTO `korisnik` (`rfid`, `slika`, `ime`, `prezime`) VALUES
('7A4B9A11\r', '', 'Ivo', 'Ivic'),
('7AD03811', '', 'Domagoj', 'Andlar'),
('8AF66011', '', 'Lukas ', 'Kristic'),
('9A328111\r', '', 'Kero', 'Keric'),
('9A328111', '', 'Matija', 'Benotic');

-- --------------------------------------------------------

--
-- Table structure for table `rfid`
--

CREATE TABLE `rfid` (
  `id_rfid` int(11) NOT NULL,
  `value` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `rfid`
--

INSERT INTO `rfid` (`id_rfid`, `value`) VALUES
(6, '9A328111'),
(7, '8AF66011'),
(8, '7AD03811'),
(9, '9A328111'),
(10, '8AF66011'),
(11, '7AD03811'),
(12, '8AF66011'),
(13, '9A328111'),
(14, '9A328111'),
(15, '7AD03811'),
(16, '7AD03811'),
(17, '7AD03811'),
(18, '7AD03811'),
(19, '7AD03811'),
(20, '7AD03811'),
(21, '9A328111'),
(22, '7AD03811'),
(23, '9A328111'),
(24, '9A328111'),
(25, '9A328111'),
(26, '9A328111'),
(27, '7AD03811'),
(28, '7AD03811'),
(29, '7AD03811'),
(30, 'hahaha');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `korisnik`
--
ALTER TABLE `korisnik`
  ADD PRIMARY KEY (`rfid`),
  ADD UNIQUE KEY `rfid` (`rfid`);

--
-- Indexes for table `rfid`
--
ALTER TABLE `rfid`
  ADD PRIMARY KEY (`id_rfid`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `rfid`
--
ALTER TABLE `rfid`
  MODIFY `id_rfid` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
