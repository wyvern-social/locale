CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(32) NOT NULL,
    discriminator CHAR(4) NOT NULL DEFAULT 0000,
    display_name VARCHAR(64),
    email VARCHAR(255) NOT NULL,
    password TEXT NOT NULL,
    birthday DATE NOT NULL,
    token_h TEXT NOT NULL,
    token_e TEXT NOT NULL,

    avatar TEXT,
    banner TEXT,
    about TEXT,
    locale VARCHAR(10) NOT NULL DEFAULT 'en_us',
    region VARCHAR(10) NOT NULL DEFAULT 'US',

    rank BIGINT DEFAULT 0,
    badges BIGINT DEFAULT 0,
    is_bot BOOLEAN DEFAULT FALSE,
    is_staff BOOLEAN DEFAULT FALSE,

    created_at TIMESTAMPTZ DEFAULT now(),
    updated_at TIMESTAMPTZ DEFAULT now()
);

CREATE TABLE name_resv (
    username VARCHAR(32) PRIMARY KEY,
    email VARCHAR(255)
);