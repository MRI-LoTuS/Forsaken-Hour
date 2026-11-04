# Forsaken Hour

Game horor roguelike first-person, single-player dan offline untuk PC Windows, dibuat dengan Unity 6. Pemain adalah mahasiswa yang terjebak di gedung kampus ITK dan harus bertahan dari kejaran **Grotesque Stalker AI** sambil mengumpulkan kunci untuk mematikan **Forsaken Clock**. Pemain memilih salah satu dari enam program studi, masing-masing dengan ability unik.

Proyek ini dikerjakan untuk mata kuliah IPPL. README ini mencatat kondisi proyek saat **presentasi progres 50%** dan hanya memuat hal yang sudah ada di repositori ini.

## Daftar isi

- [Teknologi](#teknologi)
- [Cara menjalankan](#cara-menjalankan)
- [Kontrol](#kontrol)
- [Fitur yang sudah dibuat](#fitur-yang-sudah-dibuat)
- [Belum dikerjakan](#belum-dikerjakan)
- [Perbedaan dari rencana](#perbedaan-dari-rencana)
- [Struktur proyek](#struktur-proyek)
- [Prinsip arsitektur](#prinsip-arsitektur)

## Teknologi

| Komponen | Versi |
|---|---|
| Unity | 6000.3.19f1 (Unity 6) |
| Render pipeline | Universal Render Pipeline 17.3.0 |
| Input | Input System 1.19.0 |
| Kamera | Cinemachine 3.1.7 |
| Navigasi AI | AI Navigation 2.0.13 (NavMeshSurface, NavMeshAgent) |
| UI | uGUI 2.0.0 dan TextMeshPro |

## Cara menjalankan

1. Buka proyek dengan Unity **6000.3.19f1** lewat Unity Hub.
2. Buka scene `Assets/Scenes/MainMenu.unity`, lalu tekan Play.
3. Urutan scene di Build Profiles: `MainMenu` lalu `gedung`.
4. Untuk build Windows: File > Build Profiles > Windows > Build.

Data profil disimpan sebagai `profile.json` (dengan cadangan `profile.bak`) di `Application.persistentDataPath`. Di Windows, folder ini ada di bawah `AppData\LocalLow`. File itu tidak ada di repositori.

## Kontrol

| Aksi | Tombol |
|---|---|
| Bergerak | W A S D |
| Melihat | Mouse |
| Lari | Left Shift |
| Jongkok | Left Ctrl |
| Lompat | Space |
| Interaksi (ambil item, kunci, Forsaken Clock, bersembunyi) | F |
| Inventori | E |
| Ability prodi (Night Vision) | Klik kanan mouse |
| Isi ulang baterai | R |
| Jeda | Esc |

## Fitur yang sudah dibuat

### Menu utama

- **Layar peringatan konten** saat pertama kali membuka game, dengan tombol "Saya Mengerti" yang aktif setelah hitung mundur. Status "sudah melihat" tersimpan di profil.
- **Menu utama** menampilkan currency dan prodi terpilih, dengan tombol Start Game, Study Program, Shop, Setting, dan Exit.
- **Navigasi panel** lewat `MenuNavigator` (buka, kembali, kembali ke menu utama, Esc untuk kembali).
- **Dialog konfirmasi** yang dipakai untuk keluar dari game.
- **Pengaturan**: volume master, musik, dan SFX lewat AudioMixer (`MainMixer`), sensitivitas, resolusi, layar penuh, tombol reset pengaturan, dan reset profil. Semua nilai tersimpan di profil. Catatan: nilai sensitivitas sudah tersimpan, tetapi belum dipakai oleh kamera pemain.
- **Keluar aplikasi** dengan konfirmasi (FR-09).

### Pemilihan prodi dan tampilan

- Enam prodi tersedia sebagai data (`ProdiData`): Desain Komunikasi Visual, Teknik Mesin, Teknik Kimia, Arsitektur, Teknik Elektro, dan Informatika.
- Panel **Study Program** menampilkan daftar prodi, detail (nama, status, deskripsi ability), dan tombol pilih. Prodi yang belum terbuka ditandai terkunci.
- Hanya **DKV** yang terbuka sejak awal. Prodi lain terkunci.
- Panel **Tampilan** untuk memasang kosmetik per prodi dengan tiga slot: Aksesori, Skin Tangan, dan Tampilan Senter. Tersedia tiga kosmetik contoh: Topi Kampus, Sarung Tangan Hitam (dimiliki sejak awal), dan Senter Biru.
- **Gerbang ability** (`AbilityGate`): ability hanya aktif untuk prodi yang dipilih. Saat ini baru Night Vision milik DKV yang berfungsi, jadi ability itu terkunci untuk prodi lain.

### Player Profile dan penyimpanan

- `ProfileService` adalah satu-satunya pembaca dan penulis profil. Data disimpan sebagai JSON dengan **tanda tangan SHA256** dan **file cadangan**. Perubahan manual pada file terdeteksi.
- Isi profil: currency, gedung terjauh, prodi terbuka, kosmetik dimiliki dan terpasang, prodi terpilih, dan pengaturan.
- Currency awal 500 (untuk keperluan demo).
- Logika membuka prodi, membeli dan memasang kosmetik sudah tersedia di `ProfileService` (antarmuka toko belum dibuat).
- `ProfileDebug` menyediakan menu tes di Inspector (Tambah 100 currency, Reset profil).

### Gameplay

- **Gerakan pemain**: jalan, lari, jongkok, lompat dengan `CharacterController` dan Cinemachine.
- **Interaksi**: prompt di layar untuk mengambil item, kunci, dan Forsaken Clock.
- **Inventori**: kapasitas 5 item dengan validasi kapasitas, panel inventori, dan pemakaian item.
- **Night Vision (ability DKV)**: overlay hijau, energi baterai 100 yang berkurang 5 per detik saat aktif, tidak bisa dinyalakan saat baterai habis, dan bisa diisi ulang dengan item Baterai. Item baterai tersebar di gedung.
- **Kunci dan Forsaken Clock**: 4 kunci disebar acak dari titik kandidat di gedung. Forsaken Clock hanya bisa dimatikan setelah semua kunci terkumpul, dan mematikannya berarti menang.
- **Jeda**: Esc menghentikan simulasi (termasuk Stalker) dan memunculkan panel jeda dengan opsi lanjut dan kembali ke menu.
- **Gedung ITK**: model gedung dengan NavMesh yang sudah di-bake (`NavMesh-GedITK`).

### Grotesque Stalker AI

State machine lima state di `StalkerAI`:

| State | Perilaku | Kecepatan |
|---|---|---|
| Patrol | Berpatroli antar titik patroli | 3 |
| Investigate | Mendatangi sumber suara, lalu berputar mencari | 4 |
| Hunt | Mengejar pemain saat terlihat | 5.5 |
| Lost Track | Kehilangan pemain, mencari di titik terakhir, lalu kembali ke Patrol | 2.5 |
| Catch | Menangkap pemain pada jarak tangkap (4.5) | - |

- **Penglihatan**: jarak 20 dan sudut pandang 110 derajat, dengan raycast yang memperhitungkan penghalang.
- **Pendengaran**: pemain berlari memancarkan suara lewat `PemancarSuara` dan `Kebisingan`. Stalker bereaksi bila suara berada dalam radius.
- **Bersembunyi**: pemain bisa masuk ke tempat sembunyi (misalnya lemari, `TempatSembunyi`) dengan F. Saat bersembunyi pemain tidak terlihat dan tidak mengeluarkan suara, dan interaksi lain diblokir (FR-08).
- `StalkerPatrol` adalah versi awal patroli sederhana dan sudah digantikan `StalkerAI`.

### Akhir run dan currency (FR-13)

- Saat pemain **tertangkap** atau **menang**, `RunSelesai` menghitung hadiah satu kali per run:
  - Dasar 50
  - 25 per kunci yang terkumpul
  - Bonus menang 150
- Hadiah ditambahkan ke currency lewat `ProfileService`, gedung terjauh diperbarui, dan rincian hadiah ditampilkan di panel akhir.
- Panel tertangkap dan panel menang menyediakan tombol Ulangi dan Menu Utama.


Status terhadap requirement fungsional (FR-29 dan FR-30).

| FR | Kebutuhan | Status | Bukti di repositori |
|---|---|---|---|
| FR-01 | Memulai sesi baru | Sebagian | `MainMenuPanel.Mulai`, `GameSession` (state sanity dan contamination belum ada) |
| FR-02 | Memilih appearance dan ability per prodi | Sebagian | `ProdiPanel`, `TampilanPanel`, `SlotKosmetik` (ability baru satu) |
| FR-03 | Menjeda simulasi | Selesai | `PauseMenu` (Director AI belum ada) |
| FR-04 | Melanjutkan dari kondisi terakhir | Selesai | `PauseMenu.Resume` |
| FR-05 | Menyimpan progres ke media lokal | Selesai | `ProfileService` (JSON, SHA256, cadangan) |
| FR-06 | Validasi resource dan eksekusi ability | Sebagian | `NightVision` (validasi baterai, ability lain belum) |
| FR-07 | Mengambil item dan validasi kapasitas | Selesai | `Inventory`, `Interactor`, `Pickup` |
| FR-08 | Status tersembunyi memengaruhi Stalker | Selesai | `PemainSembunyi`, `TempatSembunyi`, `StalkerAI` |
| FR-09 | Keluar aplikasi dengan konfirmasi | Selesai | `MainMenuPanel.Keluar`, `ConfirmDialog` |
| FR-10 | Pilihan prodi sebelum run | Selesai | `ProdiPanel`, `GameSession` |
| FR-11 | Ability terkunci sesuai prodi | Sebagian | `AbilityGate` (baru Night Vision DKV) |
| FR-13 | Currency saat run berakhir, termasuk kalah | Selesai | `RunSelesai` (EXP tidak ada, hanya currency) |
| FR-14 | Menyimpan Player Profile | Selesai | `PlayerProfile`, `ProfileService` |
| FR-26 | Baterai tersebar di gedung | Selesai | `Item_Baterai` di scene `gedung` |
| FR-27 | Night vision mati saat baterai habis | Sebagian | `NightVision` (tampilan UI saat menggunakan kamera belum ada)|
| FR-28 | Baterai berkurang bertahap | Selesai | `NightVision` |
| FR-29 | Kunci tersebar acak | Selesai | `KeyManager`, prefab `Key` |
| FR-30 | Forsaken Clock | Selesai | `ForsakenClock`, prefab `ForsakenClock` |

Ringkasan: **13 selesai** dan **5 sebagian** dari 30 FR.

## Belum dikerjakan

Bagian ini sudah direncanakan di GDD tetapi belum ada di repositori.

- Sanity, Contamination, dan Sanity and Contamination Director AI.
- Lima ability lain: Smoke Bomb (Teknik Kimia), Pengalih Perhatian (Teknik Mesin), Shortcut Blueprint (Arsitektur), Lampu dan Flash (Teknik Elektro), dan Akses CCTV (Informatika). Datanya sudah ada di `ProdiData`, mekanik belum.
- Toko dan layar unlock prodi dan kosmetik (FR-15). Panel Shop masih placeholder.
- Starting room berdasarkan lab prodi (FR-12). Saat ini hanya ada satu titik spawn.
- Tampilan kosmetik di dalam game. Kosmetik baru berupa data dan slot pemilihan.
- Audio dan musik adaptif. Baru ada AudioMixer dan pengaturan volume.
- Gedung selain Gedung ITK.
- Penyimpanan kondisi di tengah run (posisi dan item). Yang tersimpan baru Player Profile.

## Perbedaan dari rencana

| Hal | Rencana | Kondisi saat ini |
|---|---|---|
| Layar peringatan konten | Tidak tercantum | Ditambahkan sebelum menu utama |
| Currency awal | Tidak ditentukan | 500, sementara supaya prodi dan kosmetik bisa didemokan |
| Hadiah akhir run | EXP dan currency (SRS FR-13) | Currency saja, EXP belum ada |
| Stalker | Patrol, Investigate, Hunt, Lost Track (GDD) | Ditambah state Catch untuk proses penangkapan |

## Struktur proyek

```
Assets/
  Data/
    Prodi/            ScriptableObject enam prodi
    Kosmetik/         ScriptableObject kosmetik
    geditk/           Model dan tekstur Gedung ITK
  Resources/
    GameDatabase.asset   Daftar prodi dan kosmetik
    MainMixer.mixer      AudioMixer (VolMaster, VolMusik, VolSfx)
  Scenes/
    MainMenu.unity
    gedung.unity
    gedung/NavMesh-GedITK.asset
  Script/
    Core/             ProfileService, PlayerProfile, GameSession, RunSelesai, SettingServices, ProfileDebug, KeluarGame
    Data/             ProdiData, CosmeticData, GameDatabase
    Menu/             MenuNavigator, MenuPanel, MainMenuPanel, ContentWarningPanel, ConfirmDialog,
                      SettingPanel, ProdiPanel, KartuProdi, TampilanPanel, SlotKosmetik
    Stalker/          StalkerAI, StalkerPatrol (awal), Kebisingan, TangkapManager
                      (skrip pemain, inventori, Night Vision, kunci, clock, dan bersembunyi ada di Script/)
  prefabs/            Key, Item_Baterai, ForsakenClock, TombolItem, Menu/KartuProdi
```

## Prinsip arsitektur

- **Satu sumber kebenaran per jenis data.** Profil hanya dibaca dan ditulis oleh `ProfileService`.
- **Panel menu berdiri sendiri.** Tiap panel punya skrip sendiri dan berkomunikasi lewat `MenuNavigator` dan event `ProfileService.OnBerubah`.
- **Data disimpan sebagai id.** Prodi dan kosmetik dirujuk lewat id string dan dicari di `GameDatabase`, sehingga data mudah ditambah tanpa mengubah kode.
- **Ability dipisah dari sistem lain.** Ability dikunci lewat `AbilityGate`, sehingga ability baru tinggal ditambahkan tanpa menyentuh pemilihan prodi.
- **Suara sebagai pesan.** Pemancar suara menyiarkan lewat `Kebisingan`, dan Stalker hanya mendengarkan, sehingga keduanya tidak saling bergantung.
