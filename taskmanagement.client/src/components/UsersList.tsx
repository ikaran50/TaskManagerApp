import { useMemo } from 'react'
import type { User } from '../types'

interface Props {
    users: User[]
    filter: 'all' | 'active' | 'inactive'
    onToggle(id: number): void
    onDelete(id: number): void
}

export default function UsersList({ users, filter, onToggle, onDelete }: Props) {
    if (!users.length) return <p className="small">No users added.</p>
    const filteredList = useMemo(() => {
        let list = users
        if (filter === 'active') list = users.filter(t => t.isActive)
        if (filter === 'inactive') list = users.filter(t => !t.isActive)
        return list
    }, [users, filter])

    return (
        <div className="list">
            {filteredList.map(t => (
                <div key={t.id} className="card item">
                    <div>
                        <div className="spread">
                            <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                                {t.name}
                            </div>
                            <span className="badge">#{t.id}</span>
                        </div>
                        {t.email && <div className="small" style={{ marginTop: 4 }}>{t.email}</div>}
                        {t.isActive && <div className="small" style={{ marginTop: 4 }}>{t.isActive ? 'active' : 'inactive'}</div>}
                    </div>
                    <div className="row">
                        <button className="btn" onClick={() => onToggle(t.id)}>{t.isActive ? 'Mark Inactive' : 'Mark Active'}</button>
                        <button className="btn ghost" onClick={() => onDelete(t.id)}>Delete</button>
                    </div>
                </div>
            ))}
        </div>
    )
}