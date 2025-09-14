CREATE TABLE users (
    id CHAR(26) PRIMARY KEY,
    username VARCHAR(32) NOT NULL,
    discriminator CHAR(4) NOT NULL DEFAULT 0000,
    display_name VARCHAR(64),
    email VARCHAR(255) NOT NULL,
    password TEXT NOT NULL,
    birthday DATE NOT NULL,

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

CREATE TABLE tokens (
    id CHAR(26) PRIMARY KEY,
    user_id CHAR(26) NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token TEXT NOT NULL,
    type VARCHAR(32) NOT NULL DEFAULT 'email',
    created_at TIMESTAMPTZ DEFAULT now(),
    expires_at TIMESTAMPTZ NOT NULL,
    used BOOLEAN DEFAULT FALSE
);

CREATE TABLE waitlist (
    username VARCHAR(32) PRIMARY KEY,
    email VARCHAR(255)
);

CREATE TABLE sessions (
    id CHAR(26) PRIMARY KEY,
    user_id CHAR(26) NOT NULL REFERENCES users(id) ON DELETE CASCADE,

    refresh_token TEXT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT now(),
    expires_at TIMESTAMPTZ,
    revoked BOOLEAN DEFAULT FALSE,

    ip_address INET,
    user_agent TEXT,
)